// This is the same as the basic consola logger except with the date
// and time formatted toISOString instead of just time.
// https://github.com/nuxt/consola/blob/master/src/reporters/basic.js
export default class NhsWebReporter {
  constructor(stream) {
    this.stream = stream || process.stdout;
  }

  log(logObj) {
    const logMessages = [logObj.date.toISOString().toUpperCase()];

    if (logObj.scope) {
      logMessages.push(logObj.scope.toUpperCase());
    }

    logMessages.push(logObj.message);

    this.stream.write(`${logMessages.join(' ')}\n`);

    if (logObj.additional) {
      this.stream.write(`${logObj.additional}\n`);
    }
  }
}
