import { defineStore } from "pinia";
import { ref } from "vue";

export const useGroupsStore = defineStore("groupsStore", () => {
    const userGroups = ref([]);
    const currentGroup = ref(null);
    const groupMembers = ref([]);

    const fetchUserGroups = async (userId) => {
        try {
            const response = await fetch(
                `/CollaborativeShoppingListAPI/Groups/user/${userId}`,
                {
                    method: "GET",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                }
            );

            if (!response.ok) {
                throw new Error("Failed to fetch user groups");
            }

            userGroups.value = await response.json();
            return userGroups.value;
        } catch (error) {}
    };

    const fetchGroupInfo = async (groupId) => {
        try {
            const response = await fetch(
                `/CollaborativeShoppingListAPI/Groups/${groupId}`,
                {
                    method: "GET",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                }
            );

            if (!response.ok) {
                throw new Error("Failed to fetch user groups");
            }

            currentGroup.value = await response.json();
            return currentGroup.value;
        } catch (error) {}
    };

    const createGroup = async (groupName) => {};

    const addMemberToGroup = async (groupId, userId, role = "member") => {
        try {
            const response = await fetch(
                `/CollaborativeShoppingListAPI/GroupMembers`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${localStorage.getItem("token")}`,
                    },
                    body: JSON.stringify({
                        groupId: groupId,
                        userId: userId,
                        role: role
                    }),
                }
            );
    
            if (!response.ok) {
                const errorData = await response.json();
                console.error("Error response:", errorData);
                throw new Error(errorData.message || "Failed to add member to group");
            }
    
            // Refresh the members list
            await fetchGroupMembers(groupId);
            
            return true;
        } catch (error) {
            console.error("Error adding member to group:", error);
            throw error;
        }
    };
    
    const fetchGroupMembers = async (groupId) => {
        try {
            const response = await fetch(
                `/CollaborativeShoppingListAPI/Groups/${groupId}/members`,
                {
                    method: "GET",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                }
            );

            if (!response.ok) {
                throw new Error("Failed to fetch group members");
            }

            groupMembers.value = await response.json();
            return groupMembers.value;
        } catch (error) {
            console.error("Error fetching group members:", error);
            throw error;
        }
    };

    const removeUserFromGroup = async (groupId, userId) => {
        try {
            const response = await fetch(
                `/CollaborativeShoppingListAPI/GroupMembers/${groupId}/${userId}`,
                {
                    method: "DELETE",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                }
            );

            if (!response.ok) {
                throw new Error("Failed to remove user from group");
            }

            // Refresh the members list
            await fetchGroupMembers(groupId);
            
            return true;
        } catch (error) {
            console.error("Error removing user from group:", error);
            throw error;
        }
    };

    const deleteGroup = async (groupId) => {
        try {
            const response = await fetch(
                `/CollaborativeShoppingListAPI/Groups/${groupId}`,
                {
                    method: "DELETE",
                    headers: {
                        Authorization: `Bearer ${localStorage.getItem("token")}`,
                    },
                }
            );

            if (!response.ok) {
                throw new Error("Failed to delete group");
            }

            // Remove deleted group from userGroups
            userGroups.value = userGroups.value.filter(group => group.id !== groupId);
            if (currentGroup.value?.id === groupId) {
                currentGroup.value = null;
            }
            
            return true;
        } catch (error) {
            console.error("Error deleting group:", error);
            throw error;
        }
    };



    return {
        userGroups,
        fetchUserGroups,
        fetchGroupInfo,
        addMemberToGroup,
        removeUserFromGroup,
        fetchGroupMembers,
        deleteGroup
    };
});
