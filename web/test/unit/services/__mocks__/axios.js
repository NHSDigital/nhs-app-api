class CancelToken {

}

const axios = jest.fn(() => Promise.resolve());
axios.CancelToken = CancelToken;

export default axios;
