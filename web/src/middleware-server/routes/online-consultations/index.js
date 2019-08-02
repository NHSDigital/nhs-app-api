/* eslint-disable no-underscore-dangle */
/* eslint-disable no-param-reassign */
import Busboy from 'busboy';
import Stream from 'stream';
import { Router } from 'express';
import { noJsParameterName } from '../../../lib/noJs';
import { APPOINTMENT_ADMIN_HELP, APPOINTMENT_GP_ADVICE } from '../../../lib/routes';

export default () => {
  const router = Router();

  const handler = (req, res, next) => {
    const busboy = new Busboy({
      headers: req.headers,
      limits: {
        // deliberately 1 larger than actual limit to be able to determine if uploaded file
        // is larger than permitted while also limiting what is read from the form data
        fileSize: 1048577,
      },
    });

    let chunks = [];
    let size = 0;

    const stream = Stream.Writable();

    stream._write = (chunk, encoding, nextChunk) => {
      size += chunk.length;
      chunks.push(chunk);
      nextChunk();
    };

    busboy.on('field', (fieldname, val) => {
      if (fieldname === noJsParameterName) {
        req.body[noJsParameterName] = val;
      }
    });

    busboy.on('file', (inputName, file, fileName, encoding, mimeType) => {
      file.pipe(stream);

      file.on('end', () => {
        let base64;

        if (chunks.length > 0) {
          chunks = Buffer.concat(chunks);
          base64 = chunks.toString('base64');
        }

        req.body[inputName] = {
          name: fileName,
          base64,
          size,
          type: mimeType,
        };
      });
    });

    busboy.on('finish', () => {
      next();
    });

    req.pipe(busboy);
  };

  router.post(APPOINTMENT_ADMIN_HELP.path, handler);
  router.post(APPOINTMENT_GP_ADVICE.path, handler);

  return router;
};
