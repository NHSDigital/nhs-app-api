<script>
export default {
  methods: {
    showError() {
      throw new Error('The method "showError" has not been implemented');
    },
    getApiErrorResponse() {
      return this.$store.state.errors.apiErrors[0];
    },
    getDefaultErrorCodeKey(type) {
      if (!this.showError()) {
        return '';
      }
      return `errors.${this.getApiErrorResponse().status}.${type}`;
    },
    hasDefaultErrorCodeKey(type) {
      return this.$te(this.getDefaultErrorCodeKey(type));
    },
    hasComponentKey(type, domain) {
      return this.$te(this.getComponentKey(type, domain));
    },
    getRoutePath() {
      return this.$store.state.errors.routePath.substring(1).replace(/\//g, '.').replace(/-/g, '_');
    },
    getComponentKey(type, domain) {
      if (!this.showError()) {
        return '';
      }
      const component = this.getRoutePath();
      return `${component}.${domain}.${type}`;
    },
    getComponentErrorCodeKey(type) {
      if (!this.showError()) {
        return '';
      }
      const component = this.getRoutePath();
      return `${component}.errors.${this.getApiErrorResponse().status}.${type}`;
    },
    hasComponentErrorCodeKey(type) {
      return this.$te(this.getComponentErrorCodeKey(type));
    },
    getMessage(type) {
      const hasApiError = this.hasApiError();
      const domain = hasApiError ? 'errors' : 'noConnection';

      if (hasApiError && this.hasComponentErrorCodeKey(type)) {
        return this.$t(this.getComponentErrorCodeKey(type));
      } else if (this.hasComponentKey(type, domain)) {
        return this.$t(this.getComponentKey(type, domain));
      } else if (hasApiError && this.hasDefaultErrorCodeKey(type)) {
        return this.$t(this.getDefaultErrorCodeKey(type));
      } else if (this.$te(`${domain}.${type}`)) {
        return this.$t(`${domain}.${type}`);
      }
      return '';
    },
    hasApiError() {
      return this.$store.getters['errors/showApiError'];
    },
    hasConnectionError() {
      return this.$store.state.errors.hasConnectionProblem;
    },
  },
};
</script>
