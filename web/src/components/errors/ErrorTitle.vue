<template>
  <div>
    <slot v-if="header"/>
  </div>
</template>

<script>
export default {
  name: 'ErrorTitle',
  props: {
    title: {
      type: String,
      required: true,
    },
    header: {
      type: String,
      default: '',
    },
  },
  computed: {
    headerText() {
      return this.header ? this.$t(this.header) : this.titleText;
    },
    titleText() {
      return this.$t(this.title);
    },
  },
  created() {
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
