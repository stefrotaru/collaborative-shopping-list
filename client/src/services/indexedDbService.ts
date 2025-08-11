import { type ShoppingList, type ShoppingItem } from "../store/shoppingLists";

const DB_NAME = "CollaborativeShoppingListDB";
const DB_VERSION = 1;

const STORES = {
  SHOPPING_LISTS: "shoppingLists",
  SHOPPING_ITEMS: "shoppingItems",
} as const;

class IndexedDBService {
  private db: IDBDatabase | null = null;

  async init(): Promise<void> {
    // Skip if already initialized
    if (this.db) {
      return;
    }

    // Open (create) the database
    const request = indexedDB.open(DB_NAME, DB_VERSION);

    // Handle database creation/upgrade
    request.onupgradeneeded = (event) => {
      console.log("📒 Creating/Upgrading database structure...");
      const db = (event.target as IDBOpenDBRequest).result;

      // Create Shopping Lists store
      if (!db.objectStoreNames.contains(STORES.SHOPPING_LISTS)) {
        const shoppingListStore = db.createObjectStore(STORES.SHOPPING_LISTS, { keyPath: "guid" }); // Use GUID as primary key

        // Create indexes for faster queries
        shoppingListStore.createIndex("id", "id", { unique: true });
        shoppingListStore.createIndex("guid", "guid", { unique: true });
        shoppingListStore.createIndex("name", "name", { unique: false });
        shoppingListStore.createIndex("groupId", "groupId", { unique: false });
        shoppingListStore.createIndex("createdById", "createdById", { unique: false });
        console.log("📒 Created Shopping Lists store");
      }
    };

    // Wait for database to open
    return new Promise((resolve, reject) => {
      request.onsuccess = () => {
        this.db = request.result;
        console.log("📒 Database opened successfully");
        resolve();
      };
      request.onerror = () => {
        console.error("📒 Failed to open database:", request.error);
        reject(request.error);
      };
    });
  }

  async saveShoppingList(list: ShoppingList): Promise<void> {
    if (!this.db) {
      throw new Error("Database not initialized");
    }

    return new Promise((resolve, reject) => {
      // Start a transaction
      const transaction = this.db!.transaction(
        [STORES.SHOPPING_LISTS],
        "readwrite"
      );
      const store = transaction.objectStore(STORES.SHOPPING_LISTS);

      // Add timestamp for tracking
      const listWithTimestamp = {
        ...list,
        lastSynced: new Date(),
      };

      // Put = insert or update if exists
      const request = store.put(listWithTimestamp);

      request.onsuccess = () => {
        console.log("✅ Shopping list saved:", list.guid);
        resolve();
      };
      request.onerror = () => {
        console.error("❌ Failed to save shopping list:", request.error);
        reject(request.error);
      };
    });
  }

  async getShoppingList(guid: string): Promise<ShoppingList | null> {
    if (!this.db) throw new Error("Database not initialized");

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction(
        [STORES.SHOPPING_LISTS],
        "readonly"
      );
      const store = transaction.objectStore(STORES.SHOPPING_LISTS);
      const request = store.get(guid);

      request.onsuccess = () => {
        const result = request.result || null;
        console.log(
          result ? `✅ Found list: ${guid}` : `❌ List not found: ${guid}`
        );
        resolve(result);
      };
      request.onerror = () => {
        console.error("❌ Failed to get shopping list:", request.error);
        reject(request.error);
      };
    });
  }

  async getAllShoppingLists(): Promise<ShoppingList[]> {
    if (!this.db) throw new Error("Database not initialized");

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction(
        [STORES.SHOPPING_LISTS],
        "readonly"
      );
      const store = transaction.objectStore(STORES.SHOPPING_LISTS);
      const request = store.getAll();

      request.onsuccess = () => {
        console.log(`✅ Found ${request.result.length} lists`);
        resolve(request.result);
      };
      request.onerror = () => {
        console.error("❌ Failed to get all lists:", request.error);
        reject(request.error);
      };
    });
  }

  async clearAll(): Promise<void> {
    if (!this.db) { 
      throw new Error("Database not initialized");
    }

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction([STORES.SHOPPING_LISTS, STORES.SHOPPING_ITEMS], "readwrite");

      const listStore = transaction.objectStore(STORES.SHOPPING_LISTS);
      // const itemStore = transaction.objectStore(STORES.SHOPPING_ITEMS);

      listStore.clear();
      // itemStore.clear();

      transaction.oncomplete = () => {
        console.log("✅ All data cleared");
        resolve();
      };
      transaction.onerror = () => {
        console.error("❌ Failed to clear data:", transaction.error);
        reject(transaction.error);
      };
    });
  }

  // Check if browser supports IndexedDB
  isSupported(): boolean {
    return "indexedDB" in window;
  }

  // Close database connection
  close(): void {
    if (this.db) {
      this.db.close();
      this.db = null;
      console.log("📒 Database connection closed");
    }
  }
}

// Export singleton instance
export const indexedDBService = new IndexedDBService();

export default IndexedDBService;
