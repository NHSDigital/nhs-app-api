ARG BASE_IMAGE

FROM ${BASE_IMAGE}

USER root
WORKDIR /tmp

ARG USER_NAME
ARG USER_ID
ARG GROUP_ID

RUN addgroup --gid ${GROUP_ID} ${USER_NAME}
RUN adduser --system --uid ${USER_ID} --gid ${GROUP_ID} ${USER_NAME}

RUN usermod -a -G audio ${USER_NAME}
RUN usermod -a -G video ${USER_NAME}
RUN chown -R ${USER_NAME} /data

USER ${USER_NAME}
WORKDIR /data

RUN mkdir ~/.m2

CMD "bash"
