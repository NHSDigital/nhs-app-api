<template>
  <div>
    <slot v-if="header"/>
  </div>
</template>

<script>
import ErrorMessageMixin from '@/components/errors/ErrorMessageMixin';

export default {
  name: 'ApiErrorTitle',

  mixins: [ErrorMessageMixin],
  props: {
    title: {
      type: String,
      default: '',
    },
    header: {
      type: String,
      default: '',
    },
  },
  computed: {
    overrideStyle() {
      return this.$store.state.errors.pageSettings.errorOverrideStyles[this.statusCode];
    },
    headerText() {
      return this.header !== '' ? this.getText(this.header) : '';
    },
    titleText() {
      return this.title !== '' ? this.getText(this.title) : '';
    },
  },
  beforeMount() {
    this.updatePageTitle();
    this.updatePageHeader();
  },
  methods: {
    updatePageHeader() {
      this.$store.dispatch('header/updateHeaderText', this.headerText);
    },
    updatePageTitle() {
      this.$store.dispatch('pageTitle/updatePageTitle', this.titleText);
    },
  },
};
</script>

<style>

</style>
