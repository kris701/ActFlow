FROM node:alpine AS build
WORKDIR /usr/src/app

COPY ActFlow.Web/package.json ./

RUN npm install

COPY ActFlow.Web/. .

RUN npm run deploy --production

FROM nginx:alpine

COPY Docker/nginx.conf /etc/nginx/nginx.conf

COPY --from=build /usr/src/app/out/web/browser/ /usr/share/nginx/html

EXPOSE 8080

ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]