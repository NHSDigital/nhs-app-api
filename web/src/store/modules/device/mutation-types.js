import Sources from '@/lib/sources';

export const initialState = () => ({
  isNativeApp: false,
  source: Sources.Web,
  referrer: undefined,
});
export const UPDATE_IS_NATIVE_APP = 'UPDATE_IS_NATIVE_APP';
export const SET_SOURCE_DEVICE = 'SET_SOURCE_DEVICE';
export const INIT_DEVICE = 'INIT_DEVICE';
export const SET_APP_REFERRER = 'SET_APP_REFERRER';
