var config = {};

config.NODE_ENV = process.env.NODE_ENV || 'production';
config.API_HOST = process.env.API_HOST || 'http://localhost:4000';
config.ORGAN_DONATION_URL = process.env.ORGAN_DONATION_URL || 'https://www.organdonation.nhs.uk';
config.SYMPTOM_CHECKER_URL = process.env.SYMPTOM_CHECKER_URL || 'https://111-int2.staging.111.service.nhs.uk​';
config.PORT = process.env.PORT || 4000;

module.exports = config;