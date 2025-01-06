import { defineStore } from "pinia";
import { ref } from "vue";

export const useAuthStore = defineStore("authStore", () => {
  const authenticatedUser = ref(null);

  const login = async (email, password) => {
    try {
      const response = await fetch('CollaborativeShoppingListAPI/Users/authenticate', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      console.log(JSON.stringify({ email, password }))
  
      if (!response.ok) {
        console.log(response);
        throw new Error('Authentication failed');
      }
  
      const user = await response.json();
      authenticatedUser.value = user;

      return true;
    } catch (error) {
      // TODO: Handle registration error
      if (error instanceof Error) {
        console.log(error.message);
      }
    }

    return false;
  }

  const register = async (username, email, password) => {
      try {
        const response = await fetch('CollaborativeShoppingListAPI/Users/register', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ username, email, password }),
        });

        console.log(JSON.stringify({ username, email, password }))
  
        if (!response.ok) {
          throw new Error('Registration failed');
        }
  
        const user = await response.json();
        authenticatedUser.value = user;

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
  }

  return {
    authenticatedUser,
    login,
    register,
    logout,
  };
});
