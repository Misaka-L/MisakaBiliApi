# MisakaBiliApi
一个提供哔哩哔哩视频流解析的 Api 项目。（后续可能会有其他功能）
## 使用指南
> Swagger 文档：[https://cdnapi.misakal.xyz/swagger/index.html](https://cdnapi.misakal.xyz/swagger/index.html)  
> 示例在线部署：`https://cdnapi.misakal.xyz/` **小水管服务器，建议自行部署，不要滥用！！！** 

### 请求视频流链接
#### BV 号
```
GET /api/bilibili/video/url?bvid=BV1LP411v7Bv
GET /api/bilibili/video/url?bvid=BV1mx411M793&page=2 (获取 P3 的视频链接)
```

#### AV 号
```
GET /api/bilibili/video/url?avid=315594987
GET /api/bilibili/video/url?bvid=15627712&page=2 (获取 P3 的视频链接)
```

#### 示例响应
```json
{
   "data": {
       "url": "https://*.bilivideo.com/*",
          "format": "mp4720",
          "timelength": 222000,
          "quality": 64
   },
   "message": "",
   "code": 200
}
```
### 重定向到视频流反向代理链接
> 将通过反向代理避开反盗链，可用于嵌入式播放器（如 html video 元素）或游戏内播放器（如 VRChat）
##### BV 号
```
GET /api/bilibili/video/url/redirect?bvid=BV1LP411v7Bv
GET /api/bilibili/video/url/redirect?bvid=BV1mx411M793&page=2 (获取 P3 的视频链接)
```
#### AV 号
```
GET /api/bilibili/video/url/redirect?avid=315594987
GET /api/bilibili/video/url/redirect?bvid=15627712&page=2 (获取 P3 的视频链接)
```