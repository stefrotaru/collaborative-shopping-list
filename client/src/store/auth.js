import { defineStore } from "pinia";
import { ref } from "vue";

export const useAuthStore = defineStore("authStore", () => {
  const authenticatedUser = ref(null);
  const userGroups = ref([]);

  const login = async (email, password) => {
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
      console.log('Token found in local storage:', token);	
      try {
        const response = await fetch(`CollaborativeShoppingListAPI/Users/userinfo`, {
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
        }
      } catch (error) {
        console.error('Error retrieving user info:', error);
      }
    }
  };

  const register = async (username, email, password, avatar) => {
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

  const logout = () => {
    authenticatedUser.value = null;
    localStorage.removeItem('token');
  }

  const getUserGroups = async () => {
    if (!authenticatedUser.value) {
      return;
    }

    try {
      const response = await fetch(`CollaborativeShoppingListAPI/Groups/user/${authenticatedUser.value.id}`, {
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
    } catch (error) {}
  }

  const populateStore = async () => {
    await checkAuth();
    await getUserGroups();
  }

  return {
    authenticatedUser,
    login,
    checkAuth,
    register,
    logout,

    userGroups,
    getUserGroups,

    populateStore
  };
});
