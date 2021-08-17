import OnDemandBeginPage from '@/pages/gp-session-on-demand/begin';
import OnDemandReturnPage from '@/pages/gp-session-on-demand/return';

import {
  GP_SESSION_ON_DEMAND_BEGIN_PATH,
  GP_SESSION_ON_DEMAND_RETURN_PATH,
} from '@/router/paths';

import {
  GP_SESSION_ON_DEMAND_BEGIN_NAME,
  GP_SESSION_ON_DEMAND_RETURN_NAME,
} from '@/router/names';

import proofLevel from '@/lib/proofLevel';
import { CLEAR_SELECTED_MENU_ITEM } from '@/middleware/nativeNavigation';
import { APP_LOGIN_HELP_PATH } from '@/router/externalLinks';

export const GP_SESSION_ON_DEMAND_BEGIN = {
  path: GP_SESSION_ON_DEMAND_BEGIN_PATH,
  name: GP_SESSION_ON_DEMAND_BEGIN_NAME,
  component: OnDemandBeginPage,
  meta: {
    headerKey: '',
    titleKey: '',
    proofLevel: proofLevel.P5,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpPath: APP_LOGIN_HELP_PATH,
  },
};

export const GP_SESSION_ON_DEMAND_RETURN = {
  path: GP_SESSION_ON_DEMAND_RETURN_PATH,
  name: GP_SESSION_ON_DEMAND_RETURN_NAME,
  component: OnDemandReturnPage,
  meta: {
    isAnonymous: true,
    crumb: {},
    nativeNavigation: CLEAR_SELECTED_MENU_ITEM,
    helpPath: APP_LOGIN_HELP_PATH,
  },
};

export default [
  GP_SESSION_ON_DEMAND_BEGIN,
  GP_SESSION_ON_DEMAND_RETURN,
];
