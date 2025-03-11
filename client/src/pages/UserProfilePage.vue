<template>
  <div class="profile-page">
    <div class="profile-container">
      <div class="profile-header">
        <h1 class="page-title">Your Profile</h1>
        <Button
          v-if="!isEditMode"
          icon="pi pi-pencil"
          label="Edit Profile"
          class="p-button-rounded edit-profile-btn"
          @click="toggleEditMode"
        />
        <div v-else class="edit-mode-actions">
          <Button
            icon="pi pi-times"
            label="Cancel"
            class="p-button-rounded p-button-outlined cancel-btn"
            @click="cancelEdit"
          />
          <Button
            icon="pi pi-check"
            label="Save"
            class="p-button-rounded save-btn"
            @click="saveProfileChanges"
          />
        </div>
      </div>

      <div class="profile-card">
        <!-- Avatar Section -->
        <div class="avatar-section">
          <div class="avatar-container" :class="{ 'edit-mode': isEditMode }">
            <div v-if="!isEditMode" class="avatar-display">
              <Avatar :size="120" />
              <div class="avatar-edit-overlay" @click="toggleEditMode">
                <i class="pi pi-camera"></i>
              </div>
            </div>
            
            <div v-else class="avatar-carousel-container">
              <Button 
                icon="pi pi-chevron-left" 
                class="p-button-rounded p-button-text carousel-nav-btn prev-btn" 
                @click="previousAvatar"
                :disabled="currentPage === 0"
              />
              
              <Carousel
                :value="avatarItems"
                :numVisible="1"
                :numScroll="1"
                :circular="true"
                :showIndicators="false"
                :showNavigators="false"
                v-model:page="currentPage"
                class="avatar-carousel"
              >
                <template #item="slotProps">
                  <div class="avatar-item">
                    <img
                      :src="`/avatars/${slotProps.data.file}`"
                      :alt="`Avatar ${slotProps.data.id}`"
                      class="avatar-image"
                    />
                  </div>
                </template>
              </Carousel>
              
              <Button 
                icon="pi pi-chevron-right" 
                class="p-button-rounded p-button-text carousel-nav-btn next-btn" 
                @click="nextAvatar"
                :disabled="currentPage === avatarItems.length - 1"
              />
            </div>
            
            <div v-if="!isEditMode" class="avatar-badge">
              <i class="pi pi-verified"></i>
            </div>
          </div>
          
          <div class="user-name">{{ user.username || 'User' }}</div>
          <div class="user-role">Member since {{ formatDate(user.createdAt) }}</div>
        </div>

        <!-- Profile Info Section -->
        <div class="profile-info-section">
          <div class="profile-form">
            <div class="form-group">
              <label class="form-label">Username</label>
              <div v-if="!isEditMode" class="info-display">
                <i class="pi pi-user info-icon"></i>
                <span class="info-text">{{ user.username }}</span>
              </div>
              <div v-else class="input-wrapper">
                <i class="pi pi-user input-icon"></i>
                <InputText v-model="editedUser.username" class="p-inputtext-lg" placeholder="Enter username" />
              </div>
            </div>
            
            <div class="form-group">
              <label class="form-label">Email Address</label>
              <div v-if="!isEditMode" class="info-display">
                <i class="pi pi-envelope info-icon"></i>
                <span class="info-text">{{ user.email }}</span>
              </div>
              <div v-else class="input-wrapper">
                <i class="pi pi-envelope input-icon"></i>
                <InputText v-model="editedUser.email" class="p-inputtext-lg" placeholder="Enter email" />
              </div>
            </div>
            
            <div class="form-group">
              <label class="form-label">Account Type</label>
              <div class="info-display">
                <i class="pi pi-id-card info-icon"></i>
                <span class="info-text">{{ user.accountType || 'Standard' }}</span>
                <Badge v-if="isPremium" value="Premium" class="premium-badge" />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Activity Section -->
      <div class="activity-section">
        <h2 class="section-title">Your Activity</h2>
        
        <div class="stats-container">
          <div class="stat-card">
            <div class="stat-value">{{ userStats.lists }}</div>
            <div class="stat-label">Shopping Lists</div>
            <i class="pi pi-list stat-icon"></i>
          </div>
          
          <div class="stat-card">
            <div class="stat-value">{{ userStats.groups }}</div>
            <div class="stat-label">Groups</div>
            <i class="pi pi-users stat-icon"></i>
          </div>
          
          <div class="stat-card">
            <div class="stat-value">{{ userStats.items }}</div>
            <div class="stat-label">Items Added</div>
            <i class="pi pi-shopping-cart stat-icon"></i>
          </div>
          
          <div class="stat-card">
            <div class="stat-value">{{ userStats.completed }}</div>
            <div class="stat-label">Items Completed</div>
            <i class="pi pi-check-circle stat-icon"></i>
          </div>
        </div>
      </div>

      <!-- Account Management Section -->
      <div class="account-section">
        <h2 class="section-title">Account Management</h2>
        
        <div class="account-options">
          <div class="option-card">
            <i class="pi pi-lock option-icon"></i>
            <div class="option-content">
              <h3 class="option-title">Change Password</h3>
              <p class="option-description">Update your password to keep your account secure</p>
            </div>
            <Button 
              icon="pi pi-arrow-right" 
              class="p-button-rounded p-button-text option-button"
              @click="openChangePasswordDialog" 
            />
          </div>
          
          <div class="option-card">
            <i class="pi pi-bell option-icon"></i>
            <div class="option-content">
              <h3 class="option-title">Notification Settings</h3>
              <p class="option-description">Control how and when you receive notifications</p>
            </div>
            <Button 
              icon="pi pi-arrow-right" 
              class="p-button-rounded p-button-text option-button"
              @click="openNotificationSettings" 
            />
          </div>
          
          <div class="option-card danger">
            <i class="pi pi-trash option-icon"></i>
            <div class="option-content">
              <h3 class="option-title">Delete Account</h3>
              <p class="option-description">Permanently delete your account and all data</p>
            </div>
            <Button 
              icon="pi pi-arrow-right" 
              class="p-button-rounded p-button-text option-button"
              @click="confirmDeleteAccount" 
            />
          </div>
        </div>
      </div>
    </div>
    
    <!-- Password Change Dialog -->
    <Dialog v-model:visible="changePasswordDialog" header="Change Password" :style="{width: '90vw', maxWidth: '450px'}" :modal="true" :draggable="false">
      <div class="dialog-content">
        <div class="mb-4">
          <label for="currentPassword" class="form-label">Current Password</label>
          <div class="input-wrapper">
            <i class="pi pi-lock input-icon"></i>
            <Password id="currentPassword" v-model="passwordForm.current" :feedback="false" toggleMask class="w-full" inputClass="w-full" />
          </div>
        </div>
        
        <div class="mb-4">
          <label for="newPassword" class="form-label">New Password</label>
          <div class="input-wrapper">
            <i class="pi pi-lock input-icon"></i>
            <Password id="newPassword" v-model="passwordForm.new" toggleMask class="w-full" inputClass="w-full" />
          </div>
        </div>
        
        <div class="mb-4">
          <label for="confirmPassword" class="form-label">Confirm Password</label>
          <div class="input-wrapper">
            <i class="pi pi-lock input-icon"></i>
            <Password id="confirmPassword" v-model="passwordForm.confirm" :feedback="false" toggleMask class="w-full" inputClass="w-full" />
          </div>
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="changePasswordDialog = false" />
        <Button label="Change Password" icon="pi pi-check" class="p-button-primary" @click="changePassword" />
      </template>
    </Dialog>
    
    <!-- Delete Account Confirmation Dialog -->
    <Dialog v-model:visible="deleteAccountDialog" header="Delete Account" :style="{width: '90vw', maxWidth: '450px'}" :modal="true" :draggable="false">
      <div class="dialog-content">
        <p>Are you sure you want to delete your account? This action cannot be undone and will permanently delete:</p>
        <ul class="mt-2 mb-4 text-red-400">
          <li>Your profile and personal information</li>
          <li>All your shopping lists</li>
          <li>Your group memberships</li>
        </ul>
        
        <div class="mb-4">
          <label for="deleteConfirm" class="form-label">Type "DELETE" to confirm</label>
          <InputText id="deleteConfirm" v-model="deleteConfirmText" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button label="Cancel" icon="pi pi-times" class="p-button-text" @click="deleteAccountDialog = false" />
        <Button 
          label="Delete Account" 
          icon="pi pi-trash" 
          class="p-button-danger" 
          @click="deleteAccount"
          :disabled="deleteConfirmText !== 'DELETE'" 
        />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, nextTick } from "vue";
import { useGlobalStore } from "../store/global";
import { useAuthStore } from "../store/auth";
import { useToast } from "primevue/usetoast";
import { useRouter } from "vue-router";

import Button from "primevue/button";
import InputText from "primevue/inputtext";
import Carousel from "primevue/carousel";
import Dialog from "primevue/dialog";
import Password from "primevue/password";
import Badge from "primevue/badge";

import Avatar from "../components/Avatar.vue";
import avatarEnum from "../data/avatars";

const router = useRouter();

const globalStore = useGlobalStore();
const authStore = useAuthStore();
const toast = useToast();

// User data
const user = ref({ ...authStore.authenticatedUser });
const editedUser = ref({ ...user.value });
const isEditMode = ref(false);

// Avatar selection
const avatarItems = avatarEnum.map((avatar) => ({
  ...avatar,
  itemImageSrc: `/avatars/${avatar.file}`,
}));
const currentPage = ref(getCurrentAvatarIndex());

// Dialog states
const changePasswordDialog = ref(false);
const deleteAccountDialog = ref(false);

// Form data
const passwordForm = ref({
  current: '',
  new: '',
  confirm: ''
});
const deleteConfirmText = ref('');

// User statistics
const userStats = ref({
  lists: 0,
  groups: 0,
  items: 0,
  completed: 0
});

// Computed properties
const isPremium = computed(() => {
  return user.value.accountType === 'Premium';
});

// Helper functions
function getCurrentAvatarIndex() {
  return avatarItems.findIndex((avatar) => avatar.file === user.value?.avatar) || 0;
}

function formatDate(dateString) {
  if (!dateString) return 'Today';
  
  const date = new Date(dateString);
  return new Intl.DateTimeFormat('en-US', { 
    year: 'numeric', 
    month: 'long', 
    day: 'numeric' 
  }).format(date);
}

function previousAvatar() {
  if (currentPage.value > 0) {
    currentPage.value--;
  }
}

function nextAvatar() {
  if (currentPage.value < avatarItems.length - 1) {
    currentPage.value++;
  }
}

// Edit Action handlers
function toggleEditMode() {
  isEditMode.value = true;
  editedUser.value = { ...user.value };
}

function cancelEdit() {
  isEditMode.value = false;
  currentPage.value = getCurrentAvatarIndex();
}

async function saveProfileChanges() {
  try {
    const updatedUser = {
      ...editedUser.value,
      avatar: avatarItems[currentPage.value].file,
    };
    
    // Mock API call - replace with actual update
    await authStore.update(updatedUser);
    console.log("Profile changes to be saved:", updatedUser);
    
    // Update local user state
    user.value = { ...updatedUser };
    
    toast.add({
      severity: 'success',
      summary: 'Profile Updated',
      detail: 'Your profile has been successfully updated',
      life: 3000
    });
    
    isEditMode.value = false;
  } catch (error) {
    console.error('Error updating profile:', error);
    toast.add({
      severity: 'error',
      summary: 'Update Failed',
      detail: 'Could not update your profile',
      life: 3000
    });
  }
}

function openChangePasswordDialog() {
  passwordForm.value = {
    current: '',
    new: '',
    confirm: ''
  };
  changePasswordDialog.value = true;
}

function openNotificationSettings() {
  // Mock function - implement actual navigation or dialog
  toast.add({
    severity: 'info',
    summary: 'Coming Soon',
    detail: 'Notification settings will be available soon',
    life: 3000
  });
}

function confirmDeleteAccount() {
  deleteConfirmText.value = '';
  deleteAccountDialog.value = true;
}

async function changePassword() {
  if (passwordForm.value.new !== passwordForm.value.confirm) {
    toast.add({
      severity: 'error',
      summary: 'Passwords Do Not Match',
      detail: 'New password and confirmation do not match',
      life: 3000
    });
    return;
  }
  
  try {
    const userId = parseInt(user.value.id);
    
    if (isNaN(userId)) {
      throw new Error('Invalid user ID');
    }
    
    // Call the store function with the proper object structure and PascalCase property names
    await authStore.changePassword({
      UserId: userId,
      OldPassword: passwordForm.value.current,
      NewPassword: passwordForm.value.new
    });
    
    console.log("Password change requested for user ID:", userId);
    
    toast.add({
      severity: 'success',
      summary: 'Password Changed',
      detail: 'Your password has been successfully updated',
      life: 3000
    });
    
    changePasswordDialog.value = false;
  } catch (error) {
    console.error('Error changing password:', error);
    toast.add({
      severity: 'error',
      summary: 'Change Failed',
      detail: 'Could not update your password',
      life: 3000
    });
  }
}

async function deleteAccount() {
  try {
    const response = await fetch(`CollaborativeShoppingListAPI/Users/delete`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json'
        // Authorization: `Bearer ${localStorage.getItem('token')}`,
      },
      body: JSON.stringify({ id: user.value.id }),
    });

    if (!response.ok) {
      throw new Error('Failed to delete user account');
    }

    // Show success message
    toast.add({
      severity: 'success',
      summary: 'Account Deleted',
      detail: 'Your account has been successfully deleted',
      life: 3000
    });

    // Reset auth store
    authStore.resetStore();
    
    // Close the dialog
    deleteAccountDialog.value = false;
    
    // Navigate to login page
    router.push('/login');
    
    return true;
  }
  catch (error) {
    console.error('Error deleting user account:', error);
    
    // Show error message
    toast.add({
      severity: 'error',
      summary: 'Deletion Failed',
      detail: 'Could not delete your account. Please try again.',
      life: 3000
    });
    
    return false;
  }
}

// Fetch user statistics
async function fetchUserStats() {
  // TODO: (Mock data) replace with actual API calls
  userStats.value = {
    lists: 5,
    groups: 2,
    items: 37,
    completed: 24
  };
}

onMounted(() => {
  fetchUserStats();
});
</script>

<style lang="scss" scoped>
.profile-page .avatar-container .avatar-badge {
  bottom: 5px;
  right: 5px;
}

.avatar-carousel-container {
  .avatar-carousel {    
    :deep(.p-carousel-content) {
      overflow: visible;
    }
  }
}

.input-wrapper {  
  :deep(.p-inputtext) {
    padding-left: 2.5rem !important;
    background-color: #1e1e1e !important;
    border-color: #4b5563 !important;
    color: #e5e7eb !important;
    width: 100%;
    
    &:focus {
      border-color: #34d399 !important;
      box-shadow: 0 0 0 2px rgba(52, 211, 153, 0.2) !important;
    }
  }
}

// Dialog styles
.dialog-content {
  .input-wrapper {
    position: relative;
    
    .input-icon {
      position: absolute;
      left: 1rem;
      top: 50%;
      transform: translateY(-50%);
      color: #9ca3af;
      z-index: 1;
    }
    
    :deep(.p-password) {
      width: 100%;
      
      input {
        background-color: #1e1e1e !important;
        border-color: #4b5563 !important;
        color: #e5e7eb !important;
        padding-left: 2.5rem !important;
        width: 100%;
      }
    }
  }
}
</style>