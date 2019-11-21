export default {
  name: 'CalculateAgeInMonthsAndYears',
  methods: {
    getDisplayedAgeText(item) {
      if (item.ageYears === 0 && item.ageMonths === 0) {
        return this.$t('linkedProfiles.ageLabels.lessThanOneMonth');
      }
      if (item.ageYears === 0 && item.ageMonths === 1) {
        return item.ageMonths + this.$t('linkedProfiles.ageLabels.oneMonth');
      }
      if (item.ageYears === 0 && item.ageMonths > 1) {
        return item.ageMonths + this.$t('linkedProfiles.ageLabels.greaterThanOneMonthLessThan1Year');
      }
      if (item.ageYears === 1 && item.ageMonths >= 0) {
        return item.ageYears + this.$t('linkedProfiles.ageLabels.oneYear');
      }
      if (item.ageYears > 1 && item.ageMonths >= 0) {
        return item.ageYears + this.$t('linkedProfiles.ageLabels.greaterThanOneYearOld');
      }
      return '';
    },
  },
};
