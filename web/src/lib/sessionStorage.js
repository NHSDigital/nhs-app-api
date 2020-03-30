export default (storageKey) => {
  try {
    return !!sessionStorage.getItem(storageKey);
  } catch (e) {
    return false;
  }
};
