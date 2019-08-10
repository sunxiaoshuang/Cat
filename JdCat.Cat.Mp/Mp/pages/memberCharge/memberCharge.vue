<template>
	<view class="container">
		<view class="charge-header">
			<text style="font-size: .8em; margin-left: 15px;">我的余额</text>
			<view>
				<text style="font-size: 1.8em; margin-left: 15px;">{{balance}}</text>
				<text>元</text>
			</view>
			<view class="charge-btn">
				<button plain="true" @tap="chargeRecord()">
					<text class="jdcat jdcat-time"></text>
					<text style="margin-left: 5px;"> 充值记录</text>
				</button>
				<button plain="true" @tap="pay()">
					<text class="jdcat jdcat-rmb"></text>
					<text style="margin-left: 5px;"> 会员支付</text>
				</button>
			</view>
		</view>
		<view class="charge-content">
			<view class="store-name">
				<text class="jdcat jdcat-home" style="color: #d0a762; font-size: 1.1em;"></text>
				<text style="margin-left: 5px; font-size: .9em; color: #777;">简单猫科技</text>
			</view>
			<view class="charge-amount">
				<view class="amount-item" v-for="item in chargeList" v-bind:class="{'checked': item.checked}" @tap="clickItem(item)">
					<view class="amount-tip">
						<text style="font-size: 1.2em;">{{item.amount}}</text>
						<text>元</text>
					</view>
					<view class="amount-active" v-show="item.give > 0">
						<text>赠送 {{item.give}} 元</text>
					</view>
					<text class="jdcat jdcat-liwu"></text>
				</view>
			</view>
			<view class="bonus" v-if="bonus > 0">
				<text>赠送 {{bonus}} 积分</text>
			</view>
			<button class="charge-submit" @tap="submit()">充值</button>
		</view>
	</view>
</template>

<script>
	import helper from '@/common/helper.js';
	export default {
		data() {
			return {
				cardId: undefined,
				openid: undefined,
				encrypt_code: undefined,
				member: {},
				chargeList: [],
				bonusRule: undefined,
				bonus: 0,		// 赠送的积分
				isPaying: false // 是否正在支付
			}
		},
		async onLoad(e) {
			this.cardId = e.card_id;
			this.openid = e.openid;
			this.encrypt_code = e.encrypt_code;
			this.member = await this.getMember(e);
			var self = this;
			helper.request({
				url: `/card/cardrule?id=${e.card_id}`,
				method: "get",
				success(res) {
					var amounts = res.data.chargeRules.map(a => {
						a.checked = false;
						return a;
					});
					self.chargeList = amounts;
					self.bonusRule = res.data.bonusRule || {};
				}
			});
		},
		computed: {
			balance: function(){
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
			submit() {
				if (this.isPaying) return;
				this.isPaying = true;
				var self = this;
				var chargeRules = this.chargeList.filter(a => a.checked);
				if (chargeRules.length === 0) {
					uni.showToast({
						icon: "none",
						title: "请选择充值金额"
					})
					return;
				}
				var rule = chargeRules[0];
				var body = {
					openId: this.member.openId,
					amount: rule.amount * 100,
					give: rule.give * 100,
					bonus: this.bonus
				};
				helper.request({
					url: "/card/pay?id=" + this.member.id,
					data: body,
					method: "post",
					success(res) {
						self.isPaying = false;
						if (!res.data.success) {
							// 统一支付不成功
							uni.showToast({
								icon: "none",
								title: res.data.msg
							});
							return;
						}
						WeixinJSBridge.invoke(
							'getBrandWCPayRequest', 
							res.data.data,
							function(res) {
								if (res.err_msg == "get_brand_wcpay_request:ok") {
									uni.showToast({
										icon: "success",
										title: "支付成功"
									});
									setTimeout(a => self.chargeRecord(), 2000);
								} else {
									uni.showToast({
										icon: "none",
										title: "支付失败"
									});
								}
							}
						);
					}
				});
			},
			chargeRecord() {
				uni.redirectTo({
					url: `/pages/memberRecord/memberRecord?card_id=${this.cardId}&encrypt_code=${this.encrypt_code}&openid=${this.openid}`
				});
			},
			pay() {
				uni.redirectTo({
					url: `/pages/memberPayment/memberPayment?card_id=${this.cardId}&encrypt_code=${this.encrypt_code}&openid=${this.openid}`
				});
			},
			clickItem(item) {
				if (item.checked) return;
				this.chargeList.forEach(a => a.checked = false);
				item.checked = true;
				if (!this.bonusRule || !this.bonusRule.amount) return;
				var times = Math.floor(item.amount / this.bonusRule.amount);
				this.bonus = times * this.bonusRule.give;
			}
		}
	}
</script>

<style>
	uni-page-body {
		height: 100%;
		background-color: #f2f4f7;
	}
	
	.container {
		overflow: hidden;
	}

	.charge-header {
		display: flex;
		flex-flow: column;
		height: 150px;
		background: linear-gradient(top, #e0b766, #d0a762);
		width: 100%;
		color: #fff;
		padding: 30upx;
	}

	.charge-btn {
		display: flex;
		flex-flow: row nowrap;
		justify-content: flex-start;
	}

	.charge-btn button {
		color: #fff;
		display: inline;
		margin: 0 !important;
		margin-top: 20upx !important;
		margin-left: 30upx !important;
		padding: 14upx !important;
		line-height: 1;
		border: 1px solid #fff;
		border-radius: 30upx;
		font-size: .8em;
	}

	.charge-content {
		background: #fff;
		border-radius: 40upx;
		margin-top: -80upx;
		width: 94%;
		box-shadow: 0 2px 9px 0px rgba(76, 111, 135, 0.1);
		height: 800upx;
	}

	.store-name {
		padding: 24upx;
	}

	.charge-amount {
		display: flex;
		flex-flow: row wrap;
		justify-content: center;
		padding-bottom: 10upx;
		margin: 24upx;
	}

	.amount-item {
		position: relative;
		width: 45%;
		height: 150upx;
		border: 1px solid #ccc;
		border-radius: 10upx;
		margin: 10upx;
		display: flex;
		flex-flow: column;
		justify-content: center;
		align-items: center;
		overflow: hidden;
		font-size: .9em;
	}

	.amount-active {
		font-size: .8em;
	}

	.amount-item.checked {
		border-color: #ee903c;
	}

	.amount-item.checked .amount-active {
		color: #ee903c;
	}

	.jdcat-liwu {
		position: absolute;
		font-size: 120upx;
		top: 5upx;
		right: -18%;
		transform: rotate(-30deg);
		opacity: .4;
	}

	.amount-item.checked .jdcat-liwu {
		color: #d0a762;
		opacity: 1;
	}

	.amount {
		font-size: 1.5em;
		color: gray;
		margin-left: 20upx;
	}

	.charge-submit {
		margin: 24upx;
		background: #ee903c !important;
		color: #fff;
	}

	.bonus {
		padding-left: 40upx;
		font-size: .8em;
		color: #ee903c;
	}
</style>
