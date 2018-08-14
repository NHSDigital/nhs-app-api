<template>
  <content>
    <header v-if="showHeader" :class="[$style.header]">
      <h1 :class="[$style.h1]"> {{ $t('symptoms.checkMySymptomsHeader') }}</h1>
      <span :class="[$style.slim]">
        <a href="/login">
          <svg width="11px" height="20px" viewBox="0 0 11 20" version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" class="back">
            <title>Back</title>
            <g id="Header---Back" stroke="none" stroke-width="1" fill="none" fill-rule="evenodd"
               transform="translate(-17.000000, -15.000000)">
              <g id="icon_back_arrow" transform="translate(17.000000, 15.000000)"
                 fill="#FFFFFF" fill-rule="nonzero">
                <!--eslint-disable-next-line -->
                <path id="Shape" d="M9.76205788,19.7931034 C9.90353698,19.9310345 10.0803859,
                20 10.2749196,20 C10.4694534,20 10.6463023,19.9310345 10.7877814,
                19.7931034 C11.0707395,19.5172414 11.0707395,
                19.0689655 10.7877814,18.7931034 L1.76848875,
                10 L10.7877814,1.20689655 C11.0707395,0.931034483 11.0707395,
                0.482758621 10.7877814,0.206896552 C10.5048232,
                -0.0689655172 10.0450161,-0.0689655172 9.76205788,
                0.206896552 L0.21221865,9.5 C-0.0707395498,9.77586207 -0.0707395498,
                10.2241379 0.21221865,10.5 L9.76205788,19.7931034 Z"></path>
              </g>
            </g>
          </svg>
        </a>
      </span>
    </header>
    <main :class="[getHeaderState()]">
      <body>
        <div>
          <symptoms-check :class="$style.li"/>
        </div>
      </body>
    </main>
  </content>

</template>
<script>
/* eslint-disable import/extensions */
import symptomsCheck from '@/components/symptoms/SymptomsCheck';

export default {
  components: {
    symptomsCheck,
  },
  computed: {
    showHeader() {
      return (
        !this.$store.state.device.isNativeApp
      );
    },
  },
  methods: {
    getHeaderState() {
      return this.showHeader ? this.$style.main : this.$style.mainNoHeader;
    },
  },
};
</script>
<style module lang="scss">
  @import "../style/html";
  @import "../style/fonts";
  @import "../style/buttons";
  @import "../style/elements";

  .header {
    background: #005EB8;
    color: #ffffff;
    position: fixed;
    top: 0em;
    left: 0em;
    right: 0em;
    height: 3.125em;
    box-shadow: 0em 0em 0.313em rgba(0, 0, 0, 0.5);
    z-index: 1000;

      .h1 {
        display: block;
        font-family: "FrutigerLTW01-45Light", Arial, sans-serif;
        font-weight: normal;
        font-size: 1.250em;
        line-height: 1.250em;
        color: #ffffff;
        text-align: center;
        margin-top: 0.750em;
        margin-right: 1.875em;
        margin-left: 1.875em;

      }
      .slim {
        margin: 0em;
        margin-top: 0.875em;
        margin-left: 0.875em;
        height: 1.375em;
        float: left;
        position: absolute;
        top: 0em;
      }
  }

  .h2 {
      font-family: "FrutigerLTW01-65Bold", Arial, sans-serif;
      padding-bottom: 0.5em;
      padding-top: 0.5em;
      color: #ffffff;
  }
.li {
    box-sizing: border-box;
    background-position: right 1em center;
    position: relative;
    margin-bottom: 0em;
    margin-left: 0em;

    border-bottom: 1px #D8DDE0 solid;
}
.main {
  padding: 3.125em 0em 3.125em 2.0px;
}
.mainNoHeader {
  padding: 0em 0em 3.125em 2.0px;
}

</style>
