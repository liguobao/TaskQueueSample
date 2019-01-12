# 如何设计一个简单的队列服务和异步API

- API设计原则遵循 PayPal api-standards [API Design Patterns And Use Cases:asynchronous-operations](https://github.com/paypal/api-standards/blob/master/patterns.md#asynchronous-operations)

## 实现逻辑

- 创建任务,生成"request-id"存储到对应redis zset队列中

- 同时往redis channel发出任务消息, 后台任务处理服务自行处理此消息(生产者-消费者模式)

- 任务处理服务处理完消息之后,将处理结果写入redis,request-id为key,结果为value,然后从从redis zset从移除对应的"request-id"

- 获取request-id处理结果时:如果request-id能查询到对应的任务处理结果,直接返回处理完的数据; 如果request-id还在sortset队列则直接返回404 + 对应的位置n,表示还在处理中,前面还有n个请求;

时序图大概长这样:

![](https://ws1.sinaimg.cn/large/64d1e863gy1fz3r5m9x0ij20v80q277b.jpg)

## 运行环境

- dotnet core SDK 2.2 + Redis 随便一个版本

- appsettings.json中的RedisConnectionString为Redis链接字符串,本地环境运行的话配置文件为appsettings.Development.json

- dotnet run src


## 创建任务

```sh
curl -X POST \
  https://localhost:5901/v1/requests \
  -H 'cache-control: no-cache' \
  -H 'content-type: application/json' \
  -H 'prefer: respond-async' \
  -d '{
	"test": "1233444"
}'
```

- 参数说明 -d 为json数据,根据自己的要求定义就可以

- 同样的post请求,如果带上request-id且redis中存在此request-id,则被认为是同一个请求,直接返回

### prefer=respond-async时请求为异步响应,返回数据如下:

```json
{
    "rel": "self",
    "href": "/v1/requests/9f25a9c1850729b7329bb0f8519d4089",
    "method": "GET",
    "index": 11
}
```

同时header中存有"x-request-id"字段,作为本次请求的唯一标识,可以通过此id获取到返回结果

### prefer缺失或者为空时为同步响应,直接返回结果

## 获取翻译结果

```sh

curl -X GET \
  https://localhost:5901/v1/requests/69ecd2fa1a3d9cc3f659ace5fb2b5ca3 \
  -H 'cache-control: no-cache' \
  -H 'postman-token: c9961dbf-beb0-df58-0207-450c98f58308' \
  -H 'x-request-id: 69ecd2fa1a3d9cc3f659ace5fb2b5ca3'

```

如果还在队列或者处理中, http status code为404,数据为

```json
{
    "rel": "self",
    "href": "/v1/requests/69ecd2fa1a3d9cc3f659ace5fb2b5ca",
    "method": "GET",
    "index": 10
}
```

如果已经处理完成,正常返回200, 同时返回翻译内容,内容缓存1个小时

```json
{
   "code":0,
   "success":true
}
```