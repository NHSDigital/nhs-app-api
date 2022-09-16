<template>
  <div>
    <!--Card group is currently the only way to get this to act as the other groups style wise !-->
    <card-group class="nhsuk-grid-row">
      <card-group-item class="nhsuk-grid-column-full nhsuk-u-padding-bottom-5">
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
      </card-group-item>
    </card-group>
  </div>
</template>

<script>
import MenuItem from '@/components/MenuItem';
import MenuItemList from '@/components/MenuItemList';
import CardGroup from '@/components/widgets/card/CardGroup';
import CardGroupItem from '@/components/widgets/card/CardGroupItem';

export default {
  name: 'WayfinderHelpLink',
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
