import get from 'lodash/fp/get';

import MessagesPage from '@/pages/messages';
import GpMessagesPage from '@/pages/messages/gp-messages';
import GpMessagesUrgencyPage from '@/pages/messages/gp-messages/urgency';
import GpMessagesUrgencyContactYourGpPage from '@/pages/messages/gp-messages/urgency/contact-your-gp';
import GpMessagesRecipientsPage from '@/pages/messages/gp-messages/recipients';
import GpMessagesSendMessagePage from '@/pages/messages/gp-messages/send-message';
import GpMessagesViewDetailsPage from '@/pages/messages/gp-messages/view-details';
import GpMessagesDownloadAttachmentPage from '@/pages/messages/gp-messages/download-attachment';
import GpMessagesViewAttachmentPage from '@/pages/messages/gp-messages/view-attachment';
import GpMessagesDeletePage from '@/pages/messages/gp-messages/delete';
import GpMessagesDeleteSuccessPage from '@/pages/messages/gp-messages/delete-success';
import AppMessagingPage from '@/pages/messages/app-messaging';
import AppMessagingMessagePage from '@/pages/messages/app-messaging/app-message';

import breadcrumbs from '@/breadcrumbs/messages';
import {
  MESSAGES_PATH,
  GP_MESSAGES_PATH,
  GP_MESSAGES_URGENCY_PATH,
  GP_MESSAGES_URGENCY_CONTACT_GP_PATH,
  GP_MESSAGES_RECIPIENTS_PATH,
  GP_MESSAGES_CREATE_PATH,
  GP_MESSAGES_VIEW_MESSAGE_PATH,
  GP_MESSAGES_DOWNLOAD_ATTACHMENT_PATH,
  GP_MESSAGES_VIEW_ATTACHMENT_PATH,
  GP_MESSAGES_DELETE_PATH,
  GP_MESSAGES_DELETE_SUCCESS_PATH,
  HEALTH_INFORMATION_UPDATES_PATH,
  HEALTH_INFORMATION_UPDATES_MESSAGES_PATH,
} from '@/router/paths';
import {
  MESSAGES_NAME,
  GP_MESSAGES_NAME,
  GP_MESSAGES_URGENCY_NAME,
  GP_MESSAGES_URGENCY_CONTACT_GP_NAME,
  GP_MESSAGES_RECIPIENTS_NAME,
  GP_MESSAGES_CREATE_NAME,
  GP_MESSAGES_VIEW_MESSAGE_NAME,
  GP_MESSAGES_DOWNLOAD_ATTACHMENT_NAME,
  GP_MESSAGES_VIEW_ATTACHMENT_NAME,
  GP_MESSAGES_DELETE_NAME,
  GP_MESSAGES_DELETE_SUCCESS_NAME,
  HEALTH_INFORMATION_UPDATES_NAME,
  HEALTH_INFORMATION_UPDATES_MESSAGES_NAME,
} from '@/router/names';
import { LINKED_PROFILES_SHUTTER_MESSAGES } from '@/router/routes/linked-profiles';
import { MESSAGES_MENU_ITEM } from '@/middleware/nativeNavigation';

import proofLevel from '@/lib/proofLevel';
import { messagingHelpUrl } from '@/router/externalLinks';
import sjrRedirectRules from '@/router/sjrRedirectRules';
import { INDEX } from './general';

export const MESSAGES = {
  path: MESSAGES_PATH,
  name: MESSAGES_NAME,
  component: MessagesPage,
  meta: {
    headerKey: 'navigation.pages.headers.messages',
    titleKey: 'navigation.pages.titles.messages',
    proofLevel: proofLevel.P5,
    crumb: breadcrumbs.MESSAGES_CRUMB,
    helpUrl: messagingHelpUrl,
    nativeNavigation: MESSAGES_MENU_ITEM,
    redirectRules: [{
      condition: 'session/isProxying',
      value: true,
      route: LINKED_PROFILES_SHUTTER_MESSAGES,
    }],
  },
};

export const GP_MESSAGES = {
  path: GP_MESSAGES_PATH,
  name: GP_MESSAGES_NAME,
  component: GpMessagesPage,
  meta: {
    headerKey: 'navigation.pages.headers.gpMessages',
    titleKey: 'navigation.pages.titles.gpMessages',
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_URGENCY = {
  path: GP_MESSAGES_URGENCY_PATH,
  name: GP_MESSAGES_URGENCY_NAME,
  component: GpMessagesUrgencyPage,
  meta: {
    titleKey: 'navigation.pages.titles.gpMessagesUrgency',
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_URGENCY_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_URGENCY_CONTACT_YOUR_GP = {
  path: GP_MESSAGES_URGENCY_CONTACT_GP_PATH,
  name: GP_MESSAGES_URGENCY_CONTACT_GP_NAME,
  component: GpMessagesUrgencyContactYourGpPage,
  meta: {
    headerKey: 'navigation.pages.headers.gpMessagesUrgencyContactYourGp',
    titleKey: 'navigation.pages.titles.gpMessagesUrgencyContactYourGp',
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_URGENCY_CONTACT_YOUR_GP_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_RECIPIENTS = {
  path: GP_MESSAGES_RECIPIENTS_PATH,
  name: GP_MESSAGES_RECIPIENTS_NAME,
  component: GpMessagesRecipientsPage,
  meta: {
    headerKey: 'navigation.pages.headers.gpMessagesRecipients',
    titleKey: 'navigation.pages.titles.gpMessagesRecipients',
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_RECIPIENTS_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_SEND_MESSAGE = {
  path: GP_MESSAGES_CREATE_PATH,
  name: GP_MESSAGES_CREATE_NAME,
  component: GpMessagesSendMessagePage,
  meta: {
    headerKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.titles.gpMessagesCreateMessage', { name });
    },
    titleKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.titles.gpMessagesCreateMessage', { name });
    },
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_SEND_MESSAGE_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_VIEW_DETAILS = {
  path: GP_MESSAGES_VIEW_MESSAGE_PATH,
  name: GP_MESSAGES_VIEW_MESSAGE_NAME,
  component: GpMessagesViewDetailsPage,
  meta: {
    headerKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.headers.gpMessagesViewMessage', { name });
    },
    titleKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.titles.gpMessagesViewMessage', { name });
    },
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_VIEW_DETAILS_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_DOWNLOAD_ATTACHMENT = {
  path: GP_MESSAGES_DOWNLOAD_ATTACHMENT_PATH,
  name: GP_MESSAGES_DOWNLOAD_ATTACHMENT_NAME,
  component: GpMessagesDownloadAttachmentPage,
  meta: {
    headerKey: 'navigation.pages.headers.gpMessagesDownloadAttachment',
    titleKey: 'navigation.pages.titles.gpMessagesDownloadAttachment',
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_DOWNLOAD_ATTACHMENT_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_VIEW_ATTACHMENT = {
  path: GP_MESSAGES_VIEW_ATTACHMENT_PATH,
  name: GP_MESSAGES_VIEW_ATTACHMENT_NAME,
  component: GpMessagesViewAttachmentPage,
  meta: {
    titleKey: 'navigation.pages.titles.gpMessagesViewAttachment',
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_VIEW_ATTACHMENT_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [sjrRedirectRules.im1MessagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_DELETE = {
  path: GP_MESSAGES_DELETE_PATH,
  name: GP_MESSAGES_DELETE_NAME,
  component: GpMessagesDeletePage,
  meta: {
    headerKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.headers.gpMessagesDeleteMessage', { name });
    },
    titleKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.titles.gpMessagesDeleteMessage', { name });
    },
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_DELETE_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [
      sjrRedirectRules.im1MessagingDisabledRedirect,
      sjrRedirectRules.deleteMessageRedirect,
    ],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const GP_MESSAGES_DELETE_SUCCESS = {
  path: GP_MESSAGES_DELETE_SUCCESS_PATH,
  name: GP_MESSAGES_DELETE_SUCCESS_NAME,
  component: GpMessagesDeleteSuccessPage,
  meta: {
    headerKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.headers.gpMessagesDeleteMessageSuccess', { name });
    },
    titleKey: (store, i18n) => {
      const name = get('state.gpMessages.selectedMessageRecipient.name')(store);
      return i18n.t('navigation.pages.titles.gpMessagesDeleteMessageSuccess', { name });
    },
    proofLevel: proofLevel.P9,
    upliftRoute: INDEX,
    crumb: breadcrumbs.GP_MESSAGES_DELETE_SUCCESS_CRUMB,
    helpUrl: messagingHelpUrl,
    sjrRedirectRules: [
      sjrRedirectRules.im1MessagingDisabledRedirect,
      sjrRedirectRules.deleteMessageRedirect,
    ],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const HEALTH_INFORMATION_UPDATES = {
  path: HEALTH_INFORMATION_UPDATES_PATH,
  name: HEALTH_INFORMATION_UPDATES_NAME,
  component: AppMessagingPage,
  meta: {
    headerKey: 'navigation.pages.headers.healthAndInformationUpdates',
    titleKey: 'navigation.pages.titles.healthAndInformationUpdates',
    crumb: breadcrumbs.HEALTH_INFORMATION_UPDATES_CRUMB,
    helpUrl: messagingHelpUrl,
    proofLevel: proofLevel.P5,
    sjrRedirectRules: [sjrRedirectRules.messagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export const HEALTH_INFORMATION_UPDATES_MESSAGES = {
  path: HEALTH_INFORMATION_UPDATES_MESSAGES_PATH,
  name: HEALTH_INFORMATION_UPDATES_MESSAGES_NAME,
  component: AppMessagingMessagePage,
  meta: {
    titleKey: 'navigation.pages.titles.healthAndInformationUpdates',
    crumb: breadcrumbs.HEALTH_INFORMATION_UPDATES_MESSAGES_CRUMB,
    helpUrl: messagingHelpUrl,
    proofLevel: proofLevel.P5,
    sjrRedirectRules: [sjrRedirectRules.messagingDisabledRedirect],
    nativeNavigation: MESSAGES_MENU_ITEM,
  },
};

export default [
  MESSAGES,
  GP_MESSAGES,
  GP_MESSAGES_URGENCY,
  GP_MESSAGES_URGENCY_CONTACT_YOUR_GP,
  GP_MESSAGES_RECIPIENTS,
  GP_MESSAGES_SEND_MESSAGE,
  GP_MESSAGES_VIEW_DETAILS,
  GP_MESSAGES_DOWNLOAD_ATTACHMENT,
  GP_MESSAGES_VIEW_ATTACHMENT,
  GP_MESSAGES_DELETE,
  GP_MESSAGES_DELETE_SUCCESS,
  HEALTH_INFORMATION_UPDATES,
  HEALTH_INFORMATION_UPDATES_MESSAGES,
];
