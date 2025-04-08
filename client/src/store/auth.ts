import { defineStore } from "pinia";
import { ref } from "vue";

//TODO: Create separate file for interfaces
interface User {
  id: number;
  username: string;
  email: string;
  avatar: string;
  token: string;
}

interface UserStats {
  userId: string;
  totalLists: number;
  totalGroups: number;
  totalItemsAdded: number;
  totalItemsCompleted: number;
}

interface LoginCredentials {
  email: string;
  password: string;
}

interface RegisterCredentials {
  username: string;
  email: string;
  password: string;
  avatar?: string;
}

export const useAuthStore = defineStore("authStore", () => {
  const authenticatedUser = ref<User | null>(null);
  const userGroups = ref<any[]>([]); //TODO: Create interface for group
  const userStats = ref<UserStats | null>(null);

  const login = async ({email, password}: LoginCredentials) => {
    try {
      const response = await fetch('CollaborativeShoppingListAPI/Users/authenticate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });
  
      if (!response.ok) {
        throw new Error('Authentication failed');
      }
  
      const user = await response.json();
      authenticatedUser.value = user;

      // Save the token in local storage
      console.log(user);
      localStorage.setItem('token', user.token);

      return true;
    } catch (error) {
      // TODO: Handle registration error
      if (error instanceof Error) {
        console.log(error.message);
      }
    }

    return false;
  }

  const checkAuth = async () => {
    const token = localStorage.getItem('token');
    if (token) {
      console.log('🗼 Token found in local storage:', token);	
      try {
        const response = await fetch(`/CollaborativeShoppingListAPI/Users/userinfo`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ token }),
        });

        console.log(response);
  
        if (response.ok) {
          console.log('User is authenticated');
          const user = await response.json();
          authenticatedUser.value = user;
        } else {
          // Token is invalid or expired, remove it from local storage
          // localStorage.removeItem('token');
          console.log('checkAuth ❌ !response.ok:', response);
        }
      } catch (error) {
        console.error('Error retrieving user info:', error);
      }
    }
  };

  const registerUser = async ({username, email, password, avatar}: RegisterCredentials) => {
      try {
        const response = await fetch('CollaborativeShoppingListAPI/Users/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ username, email, password, avatar }),
        });

        console.log(JSON.stringify({ username, email, password, avatar }))
  
        if (!response.ok) {
          throw new Error('Registration failed');
        }
  
        const user = await response.json();
        authenticatedUser.value = user;
        localStorage.setItem('token', user.token);
        
        return true;
      } catch (error) {
        // TODO: Handle registration error
        if (error instanceof Error) {
          console.log(error.message);
        }
      }

      return false;
  }

  const updateUser = async (user: User) => {
    try {
      const response = await fetch('CollaborativeShoppingListAPI/Users/update', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
          // Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
        body: JSON.stringify(user),
      });

      if (!response.ok) {
        throw new Error('Failed to update user profile');
      }

      authenticatedUser.value = await response.json();
      return true;
    }
    catch (error) {
      console.error('Error updating user profile:', error);
    }
  }

  const changePassword = async (passwordData: { UserId: number, OldPassword: string, NewPassword: string }) => {
    try {
      const response = await fetch('CollaborativeShoppingListAPI/Users/changePassword', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
          // Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
        body: JSON.stringify(passwordData),
      });
  
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Failed to change password');
      }
  
      return true;
    }
    catch (error) {
      console.error('Error changing password:', error);
      throw error;
    }
  }

  const logout = () => {
    authenticatedUser.value = null;
    localStorage.removeItem('token');
  }

  const getUserGroups = async (includeAllAccessible = false) => {
    if (!authenticatedUser.value) {
      return [];
    }
  
    try {
      let url = `/CollaborativeShoppingListAPI/Groups/user/${authenticatedUser.value.id}`;
      
      // If we want all accessible groups, add a query parameter
      if (includeAllAccessible) {
        url += '?includeAllAccessible=true';
      }
      
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`,
        },
      });
  
      if (!response.ok) {
        throw new Error('Failed to fetch user groups');
      }
  
      userGroups.value = await response.json();
      return userGroups.value;
    } catch (error) {
      console.error("Error fetching user groups:", error);
      return [];
    }
  }

  const getUserStats = async () => {
    if (!authenticatedUser.value) {
      userStats.value = null;
      return null;
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        throw new Error('No token available');
      }

      const response = await fetch(`CollaborativeShoppingListAPI/Users/getUserStats?token=${token}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('Failed to fetch user stats');
      }

      const stats = await response.json();
      userStats.value = stats;
      return stats;
    } catch (error) {
      console.error('Error fetching user stats:', error);
      return null;
    }
  };

  const populateStore = async () => {
    await checkAuth();
    if (authenticatedUser.value) {
      await getUserGroups(true); // Fetch all accessible groups
      await getUserStats();
    }
  }

  const resetStore = () => {
    authenticatedUser.value = null;
    userGroups.value = [];
    localStorage.removeItem('token');
  };

  return {
    authenticatedUser,
    login,
    checkAuth,
    registerUser,
    updateUser,
    changePassword,
    logout,

    userGroups,
    getUserGroups,
    getUserStats,

    populateStore,
    resetStore
  };
});
