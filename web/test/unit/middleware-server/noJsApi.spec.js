import express from 'express';
import bodyParser from 'body-parser';
import { expressMock } from './__mocks__/express';
// eslint-disable-next-line import/first
import noJsApi from '@/middleware-server/noJsApi';

const { app } = expressMock;

describe('middleware-server/noJsApi', () => {
  it('will call the express function', () => {
    expect(express).toBeCalled();
  });

  it('will make express use the appointment routes', () => {
    expect(express().use).toBeCalled();
  });

  it('will make express use the URL encoded body parser', () => {
    expect(bodyParser.urlencoded).toHaveBeenCalled();
  });

  it('will return an object with the correct path and the app as the handler', () => {
    const { path, handler } = noJsApi;
    expect(path).toEqual('/nojs');
    expect(handler).toBe(app);
  });
});
