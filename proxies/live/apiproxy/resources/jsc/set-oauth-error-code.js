var faultCode = context.getVariable('faultcode');

context.setVariable('errorcode', faultCode === 'keymanagement.service.access_token_expired' ? 'expired' : 'forbidden');
