<template>
  <div>
    <h2 :class="$style['pharmacy-name']"> {{ pharmacy.pharmacyName }} </h2>
    <p id="address"> {{ formatAddress(pharmacy) }} </p>
    <p id="phoneNumber"> {{ pharmacy.telephoneNumber }} </p>
  </div>
</template>

<script>
export default {
  props: {
    pharmacy: {
      type: Object,
      required: true,
    },
  },
  methods: {
    formatAddress(pharmacy) {
      return [
        pharmacy.addressLine1,
        pharmacy.addressLine2,
        pharmacy.addressLine3,
        pharmacy.county,
        pharmacy.city,
        pharmacy.postcode,
      ].filter(Boolean).join(', ');
    },
  },
};
</script>

<style module lang="scss" scoped>
@import '../../style/listmenu';
@import "../../style/panels";
@import "../../style/colours";
@import "../../style/textstyles";
@import "../../style/home";

.pharmacy-name {
  margin-top: 0.5em;
  margin-bottom: 0.2em;
}

.additional-padding {
  margin-top: 1em;
}
</style>
