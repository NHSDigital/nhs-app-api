<template>
  <li :class="$style.listMenuItem">
    <analytics-tracked-tag :id="id"
                           :href="href"
                           :text="text"
                           :aria-label="ariaLabel || text"
                           :tag="tag"
                           :target="target"
                           :class="$style.listMenuItemLink"
                           :click-param="clickParam"
                           :prevent-default="preventDefault"
                           :click-func="clickFunc">
      <div :class="[$style.listMenuItemContainer, showMeta ? $style['withMeta'] : '']">
        <component :is="headerTag"
                   :class="['nhsuk-heading-s', $style.break]"
                   :aria-label="ariaText">{{ text }}</component>

        <p v-if="description"
           :id="descriptionId"
           :data-sid="descriptionDataSid">{{ description }}</p>

        <div v-if="showMeta" :class="$style['listMenuItem__meta']">
          <span v-if="showCount"
                :id="`${id}_countIndicator`"
                :class="$style['listMenuItem__count']"
                aria-hidden="true">{{ count }}</span>
          <span v-if="showIndicator"
                :id="`${id}_discIndicator`"
                :class="$style['listMenuItem__disc']"/>
        </div>
        <slot/>
      </div>
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
    showIndicator: {
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
    showMeta() {
      return this.showCount || this.showIndicator;
    },
    ariaText() {
      if (this.showIndicator) {
        return `${this.text}.${this.$t('messages.youHaveUnreadMessages')}`;
      }
      return this.showCount ? `${`${this.text} (${this.count} `}${this.$t('myRecord.records')})` : this.text;
    },

  },
};
</script>
<style module lang="scss" scoped>
  @import '../style/arrow';
  @import '~nhsuk-frontend/packages/core/settings/breakpoints';
  @import '~nhsuk-frontend/packages/core/settings/colours';
  @import '~nhsuk-frontend/packages/core/settings/globals';
  @import '~nhsuk-frontend/packages/core/settings/spacing';
  @import '~nhsuk-frontend/packages/core/settings/typography';
  @import '~nhsuk-frontend/packages/core/tools/functions';
  @import '~nhsuk-frontend/packages/core/tools/ifff';
  @import '~nhsuk-frontend/packages/core/tools/sass-mq';
  @import '~nhsuk-frontend/packages/core/tools/spacing';
  @import '~nhsuk-frontend/packages/core/tools/typography';

  @mixin outlineStyle {
    outline: 4px solid transparent;
    outline-offset: -6px;
  }

  .listMenuItem {
    @include nhsuk-responsive-margin(1, "bottom");

    .listMenuItemLink {
      @include icon-arrow-left-white-background;
      @include nhsuk-responsive-padding(3);
      display: block;
      box-sizing: border-box;
      margin-left: 0;
      border-top: 1px $color_nhsuk-grey-4 solid;
      border-bottom: 1px $color_nhsuk-grey-4 solid;
      padding: nhsuk-spacing(3);
      text-decoration: none;

      @include govuk-media-query($until: tablet) {
        @include nhsuk-responsive-padding(2, "top");
        @include nhsuk-responsive-padding(2, "bottom");
      }

      &:hover {
        @include outlineStyle;
        box-shadow: 0 0 0 4px $nhsuk-link-hover-background-color inset;
      }
      &:focus {
        @include outlineStyle;
        box-shadow: 0 0 0 4px $nhsuk-link-focus-background-color inset;
      }

      .listMenuItemContainer {
        @include nhsuk-responsive-padding(5, "right");
        position: relative;

        &.withMeta{
          @include nhsuk-responsive-padding(8, "right");
        }

        h2, h3, p {
          @include nhsuk-responsive-padding(1, "top");
          @include nhsuk-responsive-padding(0, "right");
          @include nhsuk-responsive-padding(1, "bottom");
          @include nhsuk-responsive-padding(0, "left");
          margin: 0!important;
        }

        p {
          color: $nhsuk-text-color;
        }

        .listMenuItem__meta{
          position: absolute;
          right: nhsuk-spacing(4);
          top: 0;
          height: 100%;
          min-width: nhsuk-spacing(4);

          .listMenuItem__count {
            color: $nhsuk-text-color;
            display: inline-block;
            min-width: nhsuk-spacing(5);
            min-height: nhsuk-spacing(4);
            text-align: center;
            position: absolute;
            top: 50%;
            margin-top: -11px;
            line-height: nhsuk-spacing(4);
            right: nhsuk-spacing(2);
            @include govuk-media-query($until: tablet) {
              right: 0;
            }
          }

          .listMenuItem__disc {
            @include nhsuk-responsive-padding(1, "left");
            @include nhsuk-responsive-padding(1, "right");
            @include nhsuk-typography-responsive(14);
            font-weight: $nhsuk-font-bold;
            background-color: $color_nhsuk-red;
            border-radius: nhsuk-spacing(3);
            color: $nhsuk-text-color;
            display: inline-block;
            min-width: nhsuk-spacing(4);
            min-height: nhsuk-spacing(4);
            text-align: center;
            position: absolute;
            top: 50%;
            margin-top: -12px;
            @include govuk-media-query($until: tablet) {
              padding-top: 1px;
              padding-bottom: 1px;
            }
          }
        }
      }
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

  .break {
    word-break: break-word;
  }
</style>
