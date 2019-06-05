import SessionExpiryModal from '@/components/modal/content/SessionExpiryModal';

/** private modal body register */
const modalRegister = {};

/**
 * Used to register the modal content. It guards against duplication
 * @params varag list of modal component bodies that are loaded into
 * the main modal component.
 */
export const registerModalContent = (...components) =>
  components.forEach((component) => {
    if (!component.name) {
      throw new Error('Cannot register component without a name.');
    }

    if (modalRegister[component.name]) {
      throw new Error(`Cannot register duplicate component [${component.name}].`);
    }

    modalRegister[component.name] = component;
  });


/**
 * Used to lookup up the registered component by name.
 * @param componentName - vue component name.
 * @return vue component modal body.
 */
export const getModalContent = componentName => modalRegister[componentName];


// --------------------------------------
export default {
  getModalContent,
  registerModalContent,
};
// --------------------------------------


/**
 * Register your modal body here - no duplicates please
 */
registerModalContent(
  SessionExpiryModal,
);
