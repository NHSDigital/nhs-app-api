<template>
  <!-- JAWS screen reader (on IE11) doesn't recognise tag <a> with undefined href attribute value
    as a link. So role='link' is required to tell it is a link  -->
  <component :is="tag"
             :id="id"
             :href="href"
             :role="tag === 'a' && !href ? 'link': undefined"
             :target="tag === 'a' ? target : undefined"
             :tabindex="tabindex"
             @click="trackClick($event)"
             @keypress="onKeyDown($event)">
    <slot/>
  </component>
</template>

<script>
/* eslint-disable no-unreachable */
export default {
  name: 'AnalyticsTrackedTag',
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
    target: {
      type: String,
      default: undefined,
    },
    preventDefault: {
      type: Boolean,
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
    tabindex: {
      type: Number,
      default: 0,
    },
  },
  methods: {
    trackClick(evt) {
      if (global.digitalData) {
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
          if (this.preventDefault) {
            evt.preventDefault();
          }
          this.clickFunc(this.clickParam || evt);
        }
      }
    },
    getType(tagName) {
      switch (tagName) {
        case 'A':
          return 'text_link';
        case 'H2':
          return 'accordion';
        default:
          return `unhandled_tag:${tagName}`;
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

<style module lang="scss" scoped>
</style>
