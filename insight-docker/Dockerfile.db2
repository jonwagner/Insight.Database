FROM ibmcom/db2express-c:latest
ENV DB2INST1_PASSWORD=Insight!!!Test
ENV LICENSE=accept
RUN /entrypoint.sh true
USER db2inst1
RUN . ~db2inst1/sqllib/db2profile && db2sampl
USER root
