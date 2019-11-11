<template>
  <!-- eslint-disable-next-line vue/no-v-html -->
  <component :is="tag" :id="id" v-html="linkifiedContent"/>
</template>

<script>
import linkifyContent from '@/lib/linkify';
import { key, redirectTo } from '@/lib/utils';

export default {
  name: 'LinkifyContent',
  props: {
    content: {
      type: String,
      required: true,
    },
    id: {
      type: String,
      default: undefined,
    },
    tag: {
      type: String,
      default: 'div',
      validator: value => ['div', 'p'].indexOf(value.toLowerCase()) !== -1,
    },
  },
  data() {
    return {
      linkifiedContent: linkifyContent(this.content),
    };
  },
  mounted() {
    this.$nextTick(this.addListeners);
  },
  beforeDestroy() {
    this.removeListeners();
  },
  methods: {
    addListeners() {
      this.links = this.$el.getElementsByTagName('a');
      for (let i = 0; i < this.links.length; i += 1) {
        this.links[i].addEventListener('click', this.navigateTo, false);
        this.links[i].addEventListener('keypress', this.onKeyDown, false);
      }
    },
    navigateTo(event) {
      const href = event.target.getAttribute('href');
      if (href.startsWith('/') && !href.startsWith('//')) {
        event.preventDefault();
        redirectTo(this, href, null);
      }
    },
    onKeyDown(event) {
      if (event.key === key.Enter) {
        this.navigateTo(event);
      }
    },
    removeListeners() {
      for (let i = 0; i < this.links.length; i += 1) {
        this.links[i].removeEventListener('click', this.navigateTo, false);
        this.links[i].addEventListener('keypress', this.onKeyDown, false);
      }
      this.links = [];
    },
  },
};
</script>
