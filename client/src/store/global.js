import { defineStore } from "pinia";
import { ref } from "vue";

export const useGlobalStore = defineStore("globalStore", () => {
  const isSidebarVisible = ref(false);
  const breadcrumbs = ref([]);

  const toggleSidebar = () => {
    isSidebarVisible.value = !isSidebarVisible.value;
  };
  const closeSidebar = () => {
    isSidebarVisible.value = false;
  };

  const setBreadcrumbs = (newBreadcrumbs) => {
    breadcrumbs.value = newBreadcrumbs;
  };
  const clearBreadcrumbs = () => {
    breadcrumbs.value = [];
  };
  const addCrumb = (crumb) => {
    breadcrumbs.value.push(crumb);
  };
  const removeCrumb = (crumb) => {
    breadcrumbs.value = breadcrumbs.value.filter((c) => c !== crumb);
  };

  return {
    isSidebarVisible,
    toggleSidebar,
    closeSidebar,

    breadcrumbs,
    setBreadcrumbs,
    clearBreadcrumbs,
    addCrumb,
    removeCrumb
  };
});
