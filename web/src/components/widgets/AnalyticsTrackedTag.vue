<template>
  <component :is="tag" @click="trackClick($event);" @focus="focus($event)"
             @keypress="keyPress($event)">
    <slot/>
  </component>
</template>

<script>
/* eslint-disable no-unreachable */
export default {
  props: {
    id: {
      type: String,
      default: undefined,
    },
    tag: {
      type: String,
      default: 'DIV',
    },
    text: {
      type: String,
      default: undefined,
    },
    focusFunc: {
      type: Function,
      default: undefined,
    },
    keyPressFunc: {
      type: Function,
      default: undefined,
    },
    keyPressParam: {
      type: String,
      default: undefined,
    },
    clickFunc: {
      type: Function,
      default: undefined,
    },
    clickParam: {
      type: String,
      default: '',
    },
  },
  methods: {
    trackClick(evt) {
      if (window.digitalData) {
        const el = evt.currentTarget;
        const text = this.text.trim();
        const { pageName } = window.digitalData.page.pageInfo;
        const type = (el.hasAttribute('data-purpose')) ? el.getAttribute('data-purpose')
          : this.getType(el.tagName);
        const navigation = `${pageName}|${type}|${text}`;
        this.$store.dispatch('analytics/trackLink', navigation);

        if (this.clickFunc) {
          this.clickFunc(this.clickParam);
        }
      }
    },
    getType(tagName) {
      switch (tagName) {
        case 'A': return 'text_link'; break;
        case 'H2': return 'accordion'; break;
        default: return `unhandled_tag:${tagName}`; break;
      }
    },
    focus(evt) {
      if (this.focusFunc) {
        this.focusFunc(evt);
      }
    },
    keyPress(evt) {
      if (evt.key === 'Enter' && this.keyPressFunc) {
        evt.preventDefault();
        this.keyPressFunc(this.keyPressParam);
      }
    },
  },
};
</script>
