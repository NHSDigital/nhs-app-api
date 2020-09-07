export default {
  name: 'CalculateAgeInMonthsAndYears',
  methods: {
    getDisplayedAgeText(item) {
      if (item.ageYears === 0 && item.ageMonths === 0) {
        return this.$t('generic.lessThanOneMonth');
      }
      if (item.ageYears === 0 && item.ageMonths === 1) {
        return item.ageMonths + this.$t('generic.oneMonth');
      }
      if (item.ageYears === 0 && item.ageMonths > 1) {
        return item.ageMonths + this.$t('generic.greaterThanOneMonthLessThan1Year');
      }
      if (item.ageYears === 1 && item.ageMonths >= 0) {
        return item.ageYears + this.$t('generic.oneYear');
      }
      if (item.ageYears > 1 && item.ageMonths >= 0) {
        return item.ageYears + this.$t('generic.greaterThanOneYearOld');
      }
      return '';
    },
  },
};
