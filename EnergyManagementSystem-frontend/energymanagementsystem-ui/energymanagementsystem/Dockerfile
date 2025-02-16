FROM node:16 as build
WORKDIR /app
COPY package*.json /app/

RUN npm install --legacy-peer-deps
RUN npm install -g @angular/cli 

COPY . .

RUN npm run build --prod

FROM nginx:latest

COPY ./nginx.conf /etc/nginx/nginx.template
COPY --from=build /app/dist/energymanagementsystem /usr/share/nginx/html

EXPOSE 80