<template>
	<view class="container">
		<text class="title">付款码</text>
		<text class="sub-title">结账时出示会员卡</text>
		<view class="code">
			<view class="barcode">
				<image style="height: 142upx; width: 600upx;" mode="aspectFit" :src="barcode"></image>
			</view>
			<view class="code-self">
				<text>{{payCode}}</text>
			</view>
			<view class="qrcode">
				<image style="height: 231px; width: 231px;" mode="aspectFit" :src="qrcode"></image>
			</view>
			<view class="tip">
				<text class="jdcat jdcat-jinggao" style="font-size: 12px; color: gray;"> 本支付码定时更新，请勿截屏，以免影响正常使用</text>
			</view>
			<view class="divide"></view>
			<view class="balance">
				<view class="balance-text">
					<view class="jdcat jdcat-money-circle" style="font-size: 80%;white-space: pre;">
						<text style="margin-left: 5px;">余额</text>
					</view>
					<view class="jdcat jdcat-money-rmb" style="font-size: 110%;"> {{balance}}</view>
				</view>
				<button class="balance-charge" v-on:tap="charge()">充值</button>
			</view>
		</view>
	</view>
</template>

<script>
	import helper from '@/common/helper.js';
	import config from '@/config.js';
	let updateTime = Date.now();
	export default {
		data() {
			return {
				cardId: undefined,
				openid: undefined,
				encrypt_code: undefined,
				member: {},
				barcode: undefined,
				qrcode: undefined,
				payCode: undefined
			}
		},
		async onLoad(e) {
			this.cardId = e.card_id;
			this.openid = e.openid;
			this.encrypt_code = e.encrypt_code;
			this.member = await this.getMember(e);
			this.loadCode();

			this.refreshTime = setInterval(function() { // 5秒钟检查一次，如果支付码停留时间超过一分钟，则刷新支付码
				var now = Date.now();
				if (now - updateTime > 60 * 1000) {
					this.loadCode();
				}
			}.bind(this), 5000);

			this.payTime = setInterval(function() { // 每三秒钟获取一次服务器端支付结果
				this.payResult();
			}.bind(this), 3000);
		},
		onUnload() {
			clearInterval(this.refreshTime);
			clearInterval(this.payTime);
		},
		computed: {
			balance: function() {
				return this.member.balance / 100;
			}
		},
		methods: {
			async getMember(e) {
				var member = getApp().globalData.member;
				if(member) {
					return JSON.parse(JSON.stringify(member));
				}
				var promise = new Promise(function(resolve, reject) {
					helper.request({
						url: `/card/member?cardId=${e.card_id}&openId=${e.openid}`,
						success(res) {
							resolve(res);
						},
						fail(err) {
							reject(err);
						}
					});
				});
				member = (await promise).data;
				getApp().globalData.member = JSON.parse(JSON.stringify(member))
				return member;
			},
			loadCode() {
				var self = this;
				helper.request({
					url: "/card/paycode?id=" + self.member.id,
					success(res) {
						self.payCode = res.data.data;
						updateTime = Date.now();
						self.barcode = `${config.apiUrl}/card/barcode?code=${self.payCode}&t=${updateTime}`;
						self.qrcode = `${config.apiUrl}/card/qrcode?code=${self.payCode}&t=${updateTime}`;
					}
				});
			},
			charge() {
				uni.redirectTo({
					url: `/pages/memberCharge/memberCharge?card_id=${this.cardId}&encrypt_code=${this.encrypt_code}&openid=${this.openid}`
				});
			},
			payResult() {
				var self = this;
				helper.request({
					url: "/card/payResult?code=" + this.payCode,
					success(res) {
						if (res.data.status === 0) return;
						uni.redirectTo({
							url: `/pages/memberPaySuccess/memberPaySuccess?card_id=${self.cardId}&encrypt_code=${self.encrypt_code}&openid=${self.openid}`
						});
					}
				})
			},
		}
	}
</script>

<style>
	uni-page-body {
		height: 100%;
		background-color: #d0a762;
	}

	.title {
		margin-top: 20upx;
		text-align: center;
		font-size: 120%;
		font-weight: bold;
		color: #fff;
	}

	.sub-title {
		margin-top: 10upx;
		text-align: center;
		font-size: 80%;
		color: #fff;
	}

	.code {
		width: 86%;
		margin-top: 20upx;
		background-color: #fff;
		border-radius: 20upx;
	}

	.barcode {
		margin-top: 40upx;
		text-align: center;
	}

	.code-self {
		text-align: center;
		font-size: .9em;
	}

	.qrcode {
		text-align: center;
	}

	.tip {
		text-align: center;
	}

	.divide {
		position: relative;
		border-top: 1px dashed #d4d4d4;
	}

	.divide::before {
		content: "";
		position: absolute;
		width: 8px;
		height: 8px;
		background: #d0a762;
		border-radius: 50%;
		top: -4px;
		left: -4px;
	}

	.divide::after {
		content: "";
		position: absolute;
		width: 8px;
		height: 8px;
		background: #d0a762;
		border-radius: 50%;
		top: -4px;
		right: -4px;
	}

	.balance {
		display: flex;
		flex-flow: row nowrap;
		margin: 30upx;
		justify-content: space-between;
	}

	.balance-text {
		color: #d8af5e;
		margin-left: 30upx;
	}

	.balance-charge {
		text-align: right;
		margin-right: 30upx;
		background: #ee903c;
		color: #fff;
	}
</style>
