<template>
  <div class="nhsuk-u-margin-bottom-5">
    <menu-item-list>
      <menu-item :id="id"
                 header-tag="h2"
                 data-purpose="text_link"
                 :href="path"
                 :click-func="onClickHelpLink"
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
import { redirectTo } from '@/lib/utils';

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
    path: {
      type: String,
      required: true,
    },
    backLinkOverride: {
      type: String,
      default: undefined,
    },
    routeCrumb: {
      type: String,
      default: 'defaultCrumb',
    },
  },
  methods: {
    ariaLabelCaption(text, body) {
      if (this.body) {
        return `${text}.${body}`;
      }
      return this.text;
    },
    onClickHelpLink() {
      this.$store.dispatch('navigation/setBackLinkOverride', this.backLinkOverride);
      this.$store.dispatch('navigation/setRouteCrumb', this.routeCrumb);
      redirectTo(this, this.path);
    },
  },
};
</script>
