<template>
  <component :id="id" :is="tag" :href="href" @click="trackClick($event);"
             @keypress="onKeyDown($event)">
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
    clickFunc: {
      type: Function,
      default: undefined,
    },
    clickParam: {
      type: String,
      default: '',
    },
    href: {
      type: String,
      default: undefined,
    },
    destination: {
      type: String,
      default: undefined,
    },
  },
  methods: {
    trackClick(evt) {
      if (window.digitalData) {
        const el = evt.currentTarget;
        const text = this.text.trim();
        const type = (el.hasAttribute('data-purpose')) ? el.getAttribute('data-purpose')
          : this.getType(el.tagName);

        let linkDestination = '';

        if (this.destination) {
          linkDestination = this.destination;
        } else if (this.href) {
          linkDestination = this.href;
        }

        const linkParent = this.$parent.$vnode.tag.replace('vue-component-', '');
        const linkElement = this.tag.replace('a', 'hyperlink');

        const navigation = {
          linkName: text,
          linkParent,
          linkTargetType: type,
          linkDestination,
          linkElement,
        };
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
    onKeyDown(e) {
      if (e.keyCode === 13) {
        this.trackClick(e);
      }
    },
  },
};
</script>

<style>

</style>
