# RemoLink

**RemoLink** é um túnel reverso leve para expor serviços locais à internet de forma segura e controlada, sem a necessidade de abrir portas no roteador ou configurar VPN.

O projeto é inspirado em conceitos como **SSH port forwarding** e **reverse proxy**, focando em simplicidade, clareza e aprendizado prático de redes e sistemas distribuídos.

> CLI: `rlink`

---

## ✨ Principais casos de uso

- Expor uma API local (`localhost`) para a internet
- Testar webhooks (GitHub, Stripe, Mercado Pago, etc.)
- Compartilhar serviços locais temporariamente
- Acessar serviços atrás de NAT ou firewall
- Aprender como túneis reversos funcionam na prática

---

## 🏗️ Arquitetura (visão geral)

Internet  
&nbsp;&nbsp;&nbsp;|  
[ Servidor RemoLink ]  
&nbsp;&nbsp;&nbsp;|  
==== Túnel Reverso ====  
&nbsp;&nbsp;&nbsp;|  
[ Cliente rlink ]  
&nbsp;&nbsp;&nbsp;|  
[ Serviço Local (localhost:PORTA) ]

- O cliente (`rlink`) inicia a conexão com o servidor
- Um túnel persistente é mantido
- Requisições externas são encaminhadas pelo túnel até o serviço local

---

## 🚀 Exemplo de uso

```bash
rlink expose http 5000
```

Saída esperada:

```
Service exposed at:
https://<subdominio>.remolink.io
```

A URL pública passa a encaminhar requisições para:

```
http://localhost:5000
```

---

## 🔐 Segurança

- Conexão cliente ↔ servidor protegida por TLS
- Autenticação baseada em token
- Isolamento entre túneis
- Encerramento automático em caso de desconexão

> O RemoLink é um projeto educacional e não deve ser utilizado em produção sem revisão de segurança.

---

## 📦 Componentes

### Servidor RemoLink
- Gerencia túneis
- Expõe endpoints públicos
- Encaminha tráfego

### Cliente (`rlink`)
- Conecta-se ao servidor
- Mantém o túnel ativo
- Repassa dados para o serviço local

---

## 🧩 Tecnologias

- .NET
- Comunicação assíncrona (TCP / HTTP)
- TLS
- Programação orientada a streams

---

## 🛣️ Roadmap (alto nível)

### MVP
- [ ] Exposição de serviço HTTP
- [ ] Autenticação por token
- [ ] Um túnel por cliente
- [ ] CLI básica (`expose`, `status`, `stop`)

### Futuro
- [ ] Suporte a TCP genérico
- [ ] Multiplexação de conexões
- [ ] Subdomínios dedicados
- [ ] Dashboard web
- [ ] Métricas e logs

---

## 📚 Objetivo do projeto

O RemoLink foi criado como um **projeto pessoal de aprendizado**, com foco em:

- Redes e comunicação
- Túnel reverso
- Segurança
- Arquitetura de sistemas distribuídos
- Ferramentas de linha de comando

---

## 📄 Licença

MIT
