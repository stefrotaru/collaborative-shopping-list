import { ref, watch, type Ref } from 'vue';

/**
 * Interface for the return object of useDelayedLoading
*/

interface DelayedLoadingReturn {
  loading: Ref<boolean>;
  delayedLoading: Ref<boolean>;
  setLoading: (value: boolean) => void;
}

/**
 * Composable that provides a delayed loading state
 * @param delay - Delay in milliseconds before showing loading state
 * @param minDuration - Minimum duration to show loading state once triggered
 * @returns Object containing loading and delayedLoading refs and setLoading function
*/

export function useDelayedLoading(delay = 300, minDuration = 0): DelayedLoadingReturn {
  const loading = ref<boolean>(false);
  const delayedLoading = ref<boolean>(false);
  
  let delayTimeout: number | null = null;
  let minDurationTimeout: number | null = null;
  let loadingStartTime = 0;
  
  const clearTimeouts = (): void => {
    if (delayTimeout !== null) {
      window.clearTimeout(delayTimeout);
      delayTimeout = null;
    }
    
    if (minDurationTimeout !== null) {
      window.clearTimeout(minDurationTimeout);
      minDurationTimeout = null;
    }
  };
  
  watch(loading, (isLoading: boolean): void => {
    clearTimeouts();
    
    if (isLoading) {
      loadingStartTime = Date.now();
      
      delayTimeout = window.setTimeout((): void => {
        delayedLoading.value = true;
      }, delay);
    } else {
      const loadingDuration = Date.now() - loadingStartTime;
      const remainingTime = Math.max(0, minDuration - loadingDuration);
      
      if (remainingTime > 0 && delayedLoading.value) {
        minDurationTimeout = window.setTimeout((): void => {
          delayedLoading.value = false;
        }, remainingTime);
      } else {
        delayedLoading.value = false;
      }
    }
  });
  
  const setLoading = (value: boolean): void => {
    loading.value = value;
  };
  
  return {
    loading,
    delayedLoading,
    setLoading
  };
}