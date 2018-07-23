/* eslint-disable */
import NHSOnlineApi from '../services/nhsonlineapi';

export default ({ app, store, res }) => {
  app.parseCookie = (data) => {
    //NHSO-Session-Id=CfDJ8PqcUWsAbABHlcSAuUMHP6K1g99JbVDP3qCuOEF7GMRVDWpjxAifOkUjHch5JehkKzYSipVyjFCFNBHuDnhqYfCwXvQ2VIIRYR6No1CWvMgzg9TPsbji2apR7eMdkoBhvTcNDNBNXQAOiVsVOs1dg0lEGqAF0-EJeUDRU1Uixvbsziae_8PkvJ6d0z_0dnNp77ijupyT3SQ6-8zdvTySxnrtCMwHDLzmZRaQHDVhi0s9E6LllwSWSeqfdpc2APFW16c9rNt-Yifk-GmAG9VMEWg0Pl_kfwXNhp5C6_3ScEAZtiYk2DVOHGBaW3KAMezE1wuH67DPJZ4Frx1cdUFIDHHcjKxUNnF-c5EtBKyrzhiS; path=/; httponly
    if (!data) return {};

    const parts = data.split(';').map(x => x.trim());
    const cookie = {
      options: {}
    };

    for(let i = 0; i < parts.length; i++) {
      const [name, value] = parts[i].split('=');

      if(i == 0) {
        cookie.name = name;
        cookie.value = value;
      } else {
        cookie.options[name] = value || true
      }
    }

    return cookie;
  };

  app.parseCookies = (cookies = []) => cookies.map(app.parseCookie);
};
