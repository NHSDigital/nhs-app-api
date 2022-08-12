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
        return item.ageMonths + this.$t('generic.greaterThanOneMonthLessThan2Years');
      }
      if (item.ageYears > 1 && item.ageMonths >= 0) {
        return item.ageYears + this.$t('generic.greaterThanOrEqualToTwoYearsOld');
      }
      return '';
    },
  },
};
