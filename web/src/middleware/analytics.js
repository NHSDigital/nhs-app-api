import configureAnalytics from '../services/analytics-service';

export default function ({ app, store, route }) {
  return configureAnalytics(app, store, route);
}
