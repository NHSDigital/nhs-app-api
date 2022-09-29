<template>
  <div class="nhsuk-u-margin-bottom-5">
    <menu-item-list>
      <menu-item :id="id"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="href"
                 :click-func="clickFunc"
                 :text="text"
                 :description="body"
                 :aria-label="ariaLabelCaption(text,body)"/>
    </menu-item-list>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

export default {
  name: 'HelpLink',
  components: {
    MenuItem,
    MenuItemList,
    CardGroup,
    CardGroupItem,
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
