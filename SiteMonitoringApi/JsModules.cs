using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiteMonitoringApi
{
    public static class JsModules
    {
        public static string NetSniff = @"function createHAR(e,t,s,a){var r=[];return a.forEach(function(t){var s=t.request,a=t.startReply,i=t.endReply;s&&a&&i&&(s.url.match(/(^data:image\/.*)/i)||r.push({startedDateTime:s.time.toISOString(),time:i.time-s.time,request:{method:s.method,url:s.url,httpVersion:'HTTP/1.1',cookies:[],headers:s.headers,queryString:[],headersSize:-1,bodySize:-1},response:{status:i.status,statusText:i.statusText,httpVersion:'HTTP/1.1',cookies:[],headers:i.headers,redirectURL:'',headersSize:-1,bodySize:a.bodySize,content:{size:a.bodySize,mimeType:i.contentType}},cache:{},timings:{blocked:0,dns:-1,connect:-1,send:0,wait:a.time-s.time,receive:i.time-a.time,ssl:-1},pageref:e}))}),{log:{version:'1.2',creator:{name:'PhantomJS',version:phantom.version.major+'.'+phantom.version.minor+'.'+phantom.version.patch},pages:[{startedDateTime:s.toISOString(),id:e,title:t,pageTimings:{onLoad:page.endTime-page.startTime}}],entries:r}}}Date.prototype.toISOString||(Date.prototype.toISOString=function(){function e(e){return 10>e?'0'+e:e}function t(e){return 10>e?'00'+e:100>e?'0'+e:e}return this.getFullYear()+'-'+e(this.getMonth()+1)+'-'+e(this.getDate())+'T'+e(this.getHours())+':'+e(this.getMinutes())+':'+e(this.getSeconds())+'.'+t(this.getMilliseconds())+'Z'});var page=require('webpage').create(),system=require('system');1===system.args.length?(console.log('Usage: netsniff.js<some URL>'),phantom.exit(1)):(page.address=system.args[1],page.resources=[],page.onLoadStarted=function(){page.startTime=new Date},page.onResourceRequested=function(e){page.resources[e.id]={request:e,startReply:null,endReply:null}},page.onResourceReceived=function(e){'start'===e.stage&&(page.resources[e.id].startReply=e),'end'===e.stage&&(page.resources[e.id].endReply=e)},page.open(page.address,function(e){var t;'success'!==e?(console.log('FAIL to load the address'),phantom.exit(1)):(page.endTime=new Date,page.title=page.evaluate(function(){return document.title}),t=createHAR(page.address,page.title,page.startTime,page.resources),console.log(JSON.stringify(t,void 0,4)),phantom.exit())}));";

    }
}