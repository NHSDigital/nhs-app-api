<template>
  <li :class="$style.listMenuItem">
    <analytics-tracked-tag :id="id"
                           :href="href"
                           :text="text"
                           :aria-label="ariaLabel || text"
                           :tag="tag"
                           :target="target"
                           :class="[$style['no-decoration'], $style.listMenuItemLink]"
                           :click-param="clickParam"
                           :prevent-default="preventDefault"
                           :click-func="clickFunc">
      <span :class="[$style.listMenuItemContainer, showCount ? $style['countWidth'] : '']">
        <div :class="$style['internalWrapper']">
          <component :is="headerTag"
                     :class="['nhsuk-heading-s']"
                     :aria-label="ariaText">{{ text }}</component>
          <div v-if="showCount"
               id="count"
               :class="['nhsuk-u-font-weight-regular',
                        $style['count']]"
               aria-hidden="true">{{ count }}</div>
          <p v-if="description"
             :id="descriptionId"
             :data-sid="descriptionDataSid"
             class="nhsuk-u-margin-bottom-3">{{ description }}</p>
          <div v-if="hasUnreadMessages"
               :id="id+`_unreadIndicator`"
               :class="[$style['nhs-app-message__meta'], $style['count']]">
            <span id="unreadIndicator"
                  :class="$style['nhs-app-message__count']"/>
          </div>
        </div>
        <slot/>
      </span>
    </analytics-tracked-tag>
  </li>
</template>
<script>
import AnalyticsTrackedTag from '@/components/widgets/AnalyticsTrackedTag';

export default {
  name: 'MenuItem',
  components: {
    AnalyticsTrackedTag,
  },
  props: {
    id: {
      type: String,
      required: true,
    },
    href: {
      type: String,
      default: undefined,
    },
    ariaLabel: {
      type: String,
      default: undefined,
    },
    text: {
      type: String,
      default: undefined,
    },
    description: {
      type: String,
      default: undefined,
    },
    descriptionId: {
      type: String,
      default: undefined,
    },
    descriptionDataSid: {
      type: String,
      default: undefined,
    },
    tag: {
      type: String,
      default: 'a',
    },
    target: {
      type: String,
      default: undefined,
    },
    preventDefault: {
      type: Boolean,
      default: true,
    },
    headerTag: {
      type: String,
      default: 'span',
    },
    clickFunc: {
      type: Function,
      default: undefined,
    },
    clickParam: {
      type: [String, Object],
      default: undefined,
    },
    hasUnreadMessages: {
      type: Boolean,
      default: false,
    },
    count: {
      type: Number,
      default: undefined,
    },
  },
  computed: {
    showCount() {
      return this.count !== undefined;
    },
    ariaText() {
      if (this.hasUnreadMessages) {
        return `${this.text}.${this.$t('messagesHub.unreadMessages')}`;
      }
      return this.showCount ? `${`${this.text} (${this.count} `}${this.$t('myRecord.records')})` : this.text;
    },

  },
};
</script>
<style module lang="scss" scoped>
  @import '../style/arrow';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';
  @import '~nhsuk-frontend/packages/core/settings/typography';
  @import '~nhsuk-frontend/packages/core/tools/typography';

  @mixin outlineStyle {
    outline: 4px solid transparent;
    outline-offset: -6px;
  }

  .nhs-app-message__meta{
    flex-shrink: 0;
    margin-left: nhsuk-spacing(3);
    vertical-align: middle;
    margin-top: nhsuk-spacing(1);
    .nhs-app-message__count {
      @include nhsuk-responsive-padding(1, "left");
      @include nhsuk-responsive-padding(1, "right");
      @include nhsuk-typography-responsive(14);
      font-weight: $nhsuk-font-bold;
      background-color: $color_nhsuk-yellow;
      border-radius: nhsuk-spacing(3);
      color: $nhsuk-text-color;
      border: 1px solid #B58F1C;
      display: inline-block;
      min-width: nhsuk-spacing(4);
      min-height: nhsuk-spacing(4);
      text-align: center;
      @include govuk-media-query($until: tablet) {
        padding-top: 1px;
        padding-bottom: 1px;
      }
    }
  }

  .listMenuItemLink {
    @include icon-arrow-left-white-background;
    display: block;
    box-sizing: border-box;
    margin-left: 0;

    border-top: 1px $color_nhsuk-grey-4 solid;
    border-bottom: 1px $color_nhsuk-grey-4 solid;

    &:hover {
      @include outlineStyle;
      box-shadow: 0 0 0 4px $nhsuk-link-hover-background-color inset;
    }

    &:focus {
      @include outlineStyle;
      box-shadow: 0 0 0 4px $nhsuk-link-focus-background-color inset;
    }
  }

  button.listMenuItemLink {
    display: block;
    width: 100%;
    color: $nhsuk-link-color;
    text-align: left;
    font-weight: bold;
    border-left: none;
    border-right: none;
  }

  .no-decoration {
    text-decoration: none;
  }

  .listMenuItem {
    display: block;
    margin-bottom: 5px;

    .listMenuItemContainer {
      padding: 0.2em 0.5em;
      display: block;
      cursor: pointer;

      h2, p {
        padding-left:10px;
        width: 98%;
        display: inline-block;
      }

      h2 {
        margin: 0;
      }

      h2, h3, p {
        padding-left:10px;
        width: 90%;
      }

      h2, h3 {
         margin: 0;
       }

      p {
        color: $nhsuk-text-color;
      }
    }
  }

  // Please be careful when changing the below 2 classes these are needed
  // to keep the medical record count in alignment!!!
  .internalWrapper {
    display: block;
    padding-right: 28px;
    width: 100%;
    position: relative;
  }

  .count {
    color: $nhsuk-text-color;
    font-weight: normal;
    position: absolute;
    display: inline-block;
    top: 50%;
    transform: translate(-50%, -50%);
  }
  @media screen and (device-aspect-ratio: 2/3) {
    .count {
      display: none;
    }
  }

  @media screen and (device-aspect-ratio: 40/71) {
    .count {
      display: none;
    }
  }
</style>
