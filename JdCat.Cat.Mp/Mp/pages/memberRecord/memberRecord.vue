<template>
	<view class="container">
		<view class="charge-header">
			<text style="font-size: .8em; margin-left: 15px;">余额</text>
			<view>
				<text style="font-size: 1.8em; margin-left: 15px;">{{balance}}</text>
				<text>元</text>
			</view>
			<view class="charge-btn">
				<button plain="true" @tap="chargeCharge()">
					<text class="jdcat jdcat-time"></text>
					<text style="margin-left: 5px;"> 会员充值</text>
				</button>
				<button plain="true" @tap="pay()">
					<text class="jdcat jdcat-rmb"></text>
					<text style="margin-left: 5px;"> 会员支付</text>
				</button>
			</view>
		</view>
		<view class="records">
			<view class="record" v-for="item in records">
				<text class="record-time">{{item.createTime}}</text>
				<view class="record-amount">
					<text>充值 {{item.amount / 100}} 元</text>
					<text style="color: #858898;font-size: .8em;" v-if="item.give > 0">赠送 {{item.give / 100}} 元</text>
				</view>
			</view>
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
				records: [],
				paging: {
					pageIndex: 1,
					pageSize: 20
				},
				isDone: false		// 是否记录已经全部加载
			}
		},
		async onLoad(e){
			this.cardId = e.card_id;
			this.openid = e.openid;
			this.encrypt_code = e.encrypt_code;
			this.member = await this.getMember(e);
			this.loadData();
		},
		onReachBottom(){
			if(this.isDone) return;
			this.paging.pageIndex++;
			this.loadData();
		},
		computed: {
			balance(){
				return this.member.balance / 100;
			}
		},
		methods: {
			async getMember() {
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
			loadData(){
				var self = this;
				helper.request({
					url: `/card/records?id=${this.member.id}`,
					data: this.paging,
					success(res){
						if(res.data.length === 0) {
							self.isDone = true;
							return;
						}
						res.data.forEach(a => self.records.push(a));
					}
				});
			},
			chargeCharge(){
				uni.redirectTo({
					url: `/pages/memberCharge/memberCharge?card_id=${this.cardId}&encrypt_code=${this.encrypt_code}&openid=${this.openid}`
				});
			},
			pay(){
				uni.redirectTo({
					url: `/pages/memberPayment/memberPayment?card_id=${this.cardId}&encrypt_code=${this.encrypt_code}&openid=${this.openid}`
				});
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
		text-align: center;
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

	.records{
		background: #fff;
		border-radius: 40upx;
		margin-top: -80upx;
		margin-bottom: 80upx;
		width: 94%;
		box-shadow: 0 2px 9px 0px rgba(76, 111, 135, 0.1);
		min-height: 800upx;
		padding-top: 80upx;
	}
	
	.record {
		padding: 10upx;
		display: flex;
		flex-flow: row nowrap;
		justify-content: space-between;
		border-bottom: 1px solid #ccc;
		align-items: center;
	}
	.record-time {
		color: #3C3E49;
		font-size: .9em;
	}
	.record-amount {
		display: flex;
		flex-direction: column;
		align-items: flex-end;
	}
	
</style>
