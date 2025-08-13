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

      // Create Shopping Items store
      if (!db.objectStoreNames.contains(STORES.SHOPPING_ITEMS)) {
        // autoIncrement -> Auto-generate IDs
        const itemsStore = db.createObjectStore(STORES.SHOPPING_ITEMS, { keyPath: "id", autoIncrement: true });

        itemsStore.createIndex("shoppingListId", "shoppingListId", { unique: false });
        console.log("📒 Created Shopping Items store");
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

  async addShoppingItem(item: ShoppingItem): Promise<ShoppingItem> {
    if (!this.db) {
      throw new Error("Database not initialized");
    }

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction([STORES.SHOPPING_ITEMS], "readwrite");
      const store = transaction.objectStore(STORES.SHOPPING_ITEMS);
      
      const itemWithTimestamp = {...item, lastSynced: new Date()};
      
      const request = store.add(itemWithTimestamp);
      
      request.onsuccess = () => {
        // Return the item with its generated ID
        const completeItem = { ...itemWithTimestamp, id: request.result as number };
        console.log(`✅ Added item: ${item.name} (ID: ${completeItem.id})`);
        resolve(completeItem);
      };
      
      request.onerror = () => {
        console.error("❌ Failed to add item:", request.error);
        reject(request.error);
      };
    });
  }

  async updateShoppingItem(item: ShoppingItem): Promise<void> {
    if (!this.db) {
      throw new Error("Database not initialized");
    }

    if (!item.id) {
      throw new Error("Item must have an ID to update");
    }

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction([STORES.SHOPPING_ITEMS], "readwrite");
      const store = transaction.objectStore(STORES.SHOPPING_ITEMS);
      
      const itemWithTimestamp = {...item, lastSynced: new Date()};
      
      const request = store.put(itemWithTimestamp);
      
      request.onsuccess = () => {
        console.log(`✅ Updated item: ${item.name} (ID: ${item.id})`);
        resolve();
      };
      
      request.onerror = () => {
        console.error("❌ Failed to update item:", request.error);
        reject(request.error);
      };
    });
  }

  async deleteShoppingItem(itemId: number): Promise<void> {
    if (!this.db) {
      throw new Error("Database not initialized");
    }
    
    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction([STORES.SHOPPING_ITEMS], "readwrite");
      const store = transaction.objectStore(STORES.SHOPPING_ITEMS);
      
      const request = store.delete(itemId);
      
      request.onsuccess = () => {
        console.log(`✅ Deleted item with ID: ${itemId}`);
        resolve();
      };
      
      request.onerror = () => {
        console.error("❌ Failed to delete item:", request.error);
        reject(request.error);
      };
    });
  }

  async replaceAllShoppingItems(items: ShoppingItem[], shoppingListId: number): Promise<void> {
    if (!this.db) {
      throw new Error("Database not initialized");
    }

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction([STORES.SHOPPING_ITEMS], "readwrite");
      const store = transaction.objectStore(STORES.SHOPPING_ITEMS);

      // First, delete existing items for this list
      const index = store.index("shoppingListId");
      const deleteRequest = index.openCursor(IDBKeyRange.only(shoppingListId));

      deleteRequest.onsuccess = () => {
        const cursor = deleteRequest.result;
        if (cursor) {
          store.delete(cursor.primaryKey);
          cursor.continue();
        } else {
          // After deleting, add new items
          const timestamp = new Date();
          let addCount = 0;

          items.forEach((item) => {
            const itemWithTimestamp = { ...item, lastSynced: timestamp };
            const addRequest = store.add(itemWithTimestamp);
            addRequest.onsuccess = () => {
              addCount++;
              if (addCount === items.length) {
                console.log(`✅ Saved ${items.length} items for list ${shoppingListId}`);
                resolve();
              }
            };
          });

          // Handle empty list case
          if (items.length === 0) {
            console.log(`✅ Cleared items for list ${shoppingListId}`);
            resolve();
          }
        }
      };

      transaction.onerror = () => {
        console.error("❌ Failed to save items:", transaction.error);
        reject(transaction.error);
      };
    });
  }

  async getShoppingItems(shoppingListId: number): Promise<ShoppingItem[]> {
    if (!this.db) {
      throw new Error("Database not initialized");
    }

    return new Promise((resolve, reject) => {
      const transaction = this.db!.transaction([STORES.SHOPPING_ITEMS], "readonly");
      const store = transaction.objectStore(STORES.SHOPPING_ITEMS);
      const index = store.index("shoppingListId");
      const request = index.getAll(shoppingListId);

      request.onsuccess = () => {
        console.log( `✅ Found ${request.result.length} items for list ${shoppingListId}` );
        resolve(request.result);
      };
      request.onerror = () => {
        console.error("❌ Failed to get items:", request.error);
        reject(request.error);
      };
    });
  }

  // UTILITY METHODS
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
