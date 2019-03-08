<template>
  <svg id="noIcon"
       :class="[$style.no]"
       :aria-labelledby="titleId"
       view-box="0 0 0 0" >
    <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
      <g transform="translate(-78.000000, -181.000000)" stroke="#D0021B" stroke-width="2">
        <g transform="translate(16.000000, 160.000000)">
          <g transform="translate(63.000000, 22.000000)">
            <circle cx="16.5" cy="16.5" r="16.5"/>
            <path d="M24.2144078,9.41059223 L10.1892864,23.4357136" stroke-linecap="square"/>
          </g>
        </g>
      </g>
    </g>
  </svg>
</template>

<script>
export default {
  name: 'NoIcon',
  props: {
    titleId: {
      type: String,
      required: true,
    },
  },
};
</script>

<style module lang="scss" scoped>
@import "../../../style/colours";
.no {
  margin: 0 auto;
  display: inline-block;
  width:35px;
  height:35px;
}
</style>
