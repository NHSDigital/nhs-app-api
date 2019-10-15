export default () => {
  try {
    return !!sessionStorage.getItem('hasAgreedToMedicalWarning');
  } catch (e) {
    return false;
  }
};
