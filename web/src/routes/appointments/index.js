const { Router } = require('express');

const router = Router();

router.post('/nojs/appointments/book', (req, res) => {
  res.json({ test: 'test' });
});

module.exports = router;
