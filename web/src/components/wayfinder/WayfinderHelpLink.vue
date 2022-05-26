<template>
  <menu-item-list class="nhsuk-u-padding-bottom-5">
    <menu-item :id="id"
               header-tag="h2"
               data-purpose="text_link"
               :href="href"
               :click-func="clickFunc"
               :text="text"
               :description="body"
               :aria-label="ariaLabelCaption(text,body)"/>
  </menu-item-list>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';

export default {
  name: 'WayfinderHelpLink',
  components: {
    MenuItem,
    MenuItemList,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    text: {
      type: String,
      required: true,
    },
    body: {
      type: String,
      default: '',
    },
    href: {
      type: String,
      required: true,
    },
    clickFunc: {
      type: Function,
      required: true,
    },
  },
  methods: {
    ariaLabelCaption(text, body) {
      if (this.body) {
        return `${text}. ${body}`;
      }
      return this.text;
    },
  },
};
</script>
