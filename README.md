# Chat

## Chat Api

## Configurações

É importante configurar de forma adequeada as chaves secretas do JWT nos arquivos app **_appsettings.json_** e **appsettings.Development.json**.
*Nestes arquivos, é possível configurar a string de conexão **ConnectionStrings** e a seção **Jwt**.

Para gerar Hash seguro para utilizar no atributo **JWT::Key**, utilize o seguinte comando:
**node -e "console.log(require('crypto').randomBytes(32).toString('hex'))"**

Tabém não esqueça de configurar os atributos **JWT:Issuer** e **JWT::Audience**.
