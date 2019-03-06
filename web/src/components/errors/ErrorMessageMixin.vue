<script>
import get from 'lodash/fp/get';

export default {
  computed: {
    component() {
      return this.$store.state.errors.routePath.substring(1).replace(/\//g, '.').replace(/-/g, '_');
    },
    errorCode() {
      return get('$store.state.errors.apiErrors[0].error')(this);
    },
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    hasConnectionError() {
      return this.$store.state.errors.hasConnectionProblem;
    },
    statusCode() {
      return get('$store.state.errors.apiErrors[0].status')(this);
    },
  },
  methods: {
    getComponentErrorCodeKey(type) {
      if (this.hasApiError) {
        return (this.errorCode && this.getText(`${this.component}.errors.${this.statusCode}.${this.errorCode}.${type}`))
          || (this.errorCode && this.getText(`${this.component}.errors.${this.errorCode}.${type}`))
          || this.getText(`${this.component}.errors.${this.statusCode}.${type}`);
      }

      return '';
    },
    getComponentKey(type, domain) {
      return this.getText(`${this.component}.${domain}.${type}`);
    },
    getMessage(type) {
      const domain = this.hasApiError ? 'errors' : 'noConnection';

      if (this.showError()) {
        return this.getComponentErrorCodeKey(type)
          || this.getText(`${this.component}.${domain}.${type}`)
          || this.getText(`errors.${this.statusCode}.${type}`)
          || this.getText(`${domain}.${type}`);
      }

      return '';
    },
    getText(key) {
      return this.$te(key) ? this.$t(key) : '';
    },
    showError() {
      throw new Error('The method "showError" has not been implemented');
    },
  },
};
</script>
