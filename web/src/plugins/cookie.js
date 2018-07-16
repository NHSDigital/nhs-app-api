/* eslint-disable */
export default ({ app }) => {
  app.parseCookie = (data) => {
    if (!data) return {};

    const parts = data.split(';').map(x => x.trim());
    const cookie = {
      options: {}
    };

    for(let i = 0; i < parts.length; i++) {
      const [name, value] = parts[i].split('=');

      if(i === 0) {
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
