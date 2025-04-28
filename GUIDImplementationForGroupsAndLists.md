# GUID Implementation for Collaborative Shopping List

## Overview

I implemented UUID/GUID identifiers for groups and shopping lists in our application to improve security by obscuring resource IDs in URLs.

## Problem

Previously, the app used sequential integer IDs in URLs (e.g., `/groups/5`), which had two main issues:
- **Enumeration vulnerability**: Sequential IDs made it possible to guess and access other resources
- **Information disclosure**: IDs revealed the size and growth of our database

## Solution

I added GUID properties to groups and shopping lists while maintaining integer IDs as primary keys. This approach:
- Preserves database efficiency (integer PKs)
- Obfuscates identifiers in URLs (GUIDs)
- Maintains compatibility with existing code

## Implementation Details

### 1. Database Changes

```sql
-- Add GUID column to Groups table
ALTER TABLE Groups ADD Guid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
CREATE UNIQUE INDEX IX_Groups_Guid ON Groups(Guid);

-- Add GUID column to ShoppingLists table
ALTER TABLE ShoppingLists ADD Guid UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID();
CREATE UNIQUE INDEX IX_ShoppingLists_Guid ON ShoppingLists(Guid);

-- Generate GUIDs for existing records
UPDATE Groups SET Guid = NEWID();
UPDATE ShoppingLists SET Guid = NEWID();
```

### 2. API Enhancements

| Endpoint | Purpose |
|----------|---------|
| `/ShoppingLists/guid/{guid}` | Get shopping list by GUID |
| `/ShoppingItems/shoppinglist/guid/{guid}` | Get shopping items by list GUID |
| `/Groups/guid/{guid}` | Get group by GUID |
| `/AccessibleShoppingLists/can-access-by-guid/{guid}` | Check access by GUID |

### 3. Frontend Updates

```javascript
// Navigation with GUIDs
router.push(`/shoppinglists/${list.guid}`);

// List access check by GUID
const canAccessShoppingListByGuid = async (guid: string): Promise<boolean> => {
    try {
      const listResponse = await fetch(`/CollaborativeShoppingListAPI/ShoppingLists/guid/${guid}`, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
          'Accept': 'application/json'
        },
      });
      
      if (listResponse.ok) {
        return true;
      }
      
      return false;
    } catch (err) {
      console.error('Error checking shopping list access by GUID:', err);
      return false;
    }
};
```

## Benefits

- **Improved security**: Resource IDs cannot be easily guessed or enumerated
- **Minimal disruption**: No schema changes to existing tables' primary keys
- **Better scalability**: Prepared for future distributed systems that work better with GUIDs
- **Low risk approach**: Maintains all existing functionality while adding new capabilities

## Technical Approach

Rather than completely replacing integer IDs with GUIDs (which would require extensive database schema changes), I chose to add GUIDs as secondary identifiers. This approach brought security benefits of GUIDs in public-facing URLs while maintaining the performance benefits of integer primary keys in the database.
